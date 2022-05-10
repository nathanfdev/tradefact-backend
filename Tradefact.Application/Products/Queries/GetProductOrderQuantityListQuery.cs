using Core.Common;
using Core.Models.Extended;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using X.PagedList;

namespace Tradefact.Application.Products.Queries
{
    public class GetProductOrderQuantityListQuery : IRequest<IPagedList<ProductResource>>
    {
        public Guid? OrganisationId { get; set; }
        public string Search { get; set; }
        public PagedResultParameters Paging { get; set; }

        public Guid? SupplierId { get; set; }

        public string SortBy { get; set; }

    public class GetProductOrderQuantityListQueryHandler : IRequestHandler<GetProductOrderQuantityListQuery, IPagedList<ProductResource>>
        {
            private readonly IDbConnection _connection;

            public GetProductOrderQuantityListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<ProductResource>> Handle(GetProductOrderQuantityListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder where_supplier_clause = new StringBuilder();
                StringBuilder order_by_clause = new StringBuilder();
                if (request.SupplierId.HasValue) where_supplier_clause.Append($"WHERE PS.SupplierId = @SupplierId");

                order_by_clause.Append($"{SortProductsQuery(request.SortBy)}");

                string buildGet(Guid? supplierId, string search) {

                    StringBuilder where_clause = new StringBuilder();
                    if (!String.IsNullOrEmpty(request.Search) && request.Search.Length > 0) 
                        where_clause.Append($"AND (P.Nickname like @search OR P.Name like @search OR P.[Description] like @search OR p.[SKU] like @search OR p.[Tags] like @search)");

                    if (!supplierId.HasValue)
                    {
                        return @$";WITH cte_product_ids AS (
                                SELECT P.Id 
                                    FROM[dbo].Products P
                                WHERE P.CompanyId = @OrganisationId {where_clause.ToString()}
                                GROUP BY P.Id
                           )";
                    }

                    return @$";WITH cte_product_ids AS (
                                SELECT P.Id 
                                    FROM[dbo].Products P
                                        INNER JOIN[dbo].ProductSuppliers PS ON PS.ProductId = P.Id
                                WHERE P.CompanyId = @OrganisationId AND PS.SupplierId = @SupplierId {where_clause.ToString()}
                                GROUP BY P.Id
                           )";
                }

                string sQuery = @$";
                        DECLARE @Products TABLE
                        (   
                            ProductId UNIQUEIDENTIFIER NOT NULL, 
                            NoPurchaseOrders BIGINT  DEFAULT 0, 
                            OrderQty INT  DEFAULT 0, 
                            NoShipments INT  DEFAULT 0, 
                            ShipQty BIGINT DEFAULT 0, 
                            ShipQtyInTransit BIGINT DEFAULT 0, 
                            ShipNoCartons BIGINT DEFAULT 0,
                            ShipNoCartonsInTransit	BIGINT DEFAULT 0,
                            AvailableSuppliers INT DEFAULT 0
                        )

                        { buildGet(request.SupplierId, request.Search) }

                        ,cte_po_totals AS (

                            SELECT PO.ProductId, COUNT(PO.PurchaseOrderId) [NoPurchaseOrders], SUM(Qty) [OrderQty] FROM (
                                SELECT PI.ProductId, P.Id [PurchaseOrderId], SUM(PI.OrderQuantity) [Qty]
                                FROM cte_product_ids PL
                                    INNER JOIN [dbo].PurchaseOrderItems PI ON PI.ProductId = PL.Id
                                    INNER JOIN  [dbo].[PurchaseOrders] P ON P.Id =  PI.PurchaseOrderId
                                WHERE P.CompanyId = @OrganisationId AND P.Active = 1 AND (P.[Status] >= 20 and P.[Status] < 70)
                                GROUP BY PI.ProductId, P.Id
                            ) PO
                            GROUP BY PO.ProductId
                        )
                        , cte_ship_totals AS (
                            SELECT  S.ProductId, COUNT(S.Id) [NoShipments], 
                                    SUM(ShipQty) [ShipQty], 
                                    SUM(ShipNoCartons) [ShipNoCartons],
                                    SUM(ShipQty) [ShipQtyInTransit], 
                                    SUM(ShipNoCartons) [ShipNoCartonsInTransit] 
                            FROM (
                                SELECT S.Id, CI.ProductId, 
                                        SUM(CI.CartonQty) [ShipNoCartons], SUM(CI.Qty) [ShipQty],
                                        SUM(CASE WHEN S.Collected= 1 AND S.ArrivedPOD = 0 THEN CI.CartonQty ELSE 0 END) [CartonsInTransit],
                                        SUM(CASE WHEN S.Collected= 1 AND S.ArrivedPOD = 0 THEN CI.Qty ELSE 0 END) [QtyInTransit]
                                FROM cte_product_ids PI
                                    INNER JOIN [dbo].CargoItems CI ON CI.ProductId = PI.Id
                                    INNER JOIN [dbo].Shipments S ON S.FreightMovementId = CI.FreightMovementId
                                    LEFT JOIN [dbo].[Partnerships] P ON P.Id = S.PartnershipId
                                WHERE P.ClientId = @OrganisationId AND S.Active = 1
                                GROUP BY S.Id, CI.ProductId
                            ) S
                            GROUP BY S.ProductId
                        ),
                        cte_product_supplier_ids AS (
                            SELECT P.Id [ProductId], COUNT(P.Id) [SupplierProductCount]
                                FROM Products P
                                INNER JOIN [dbo].[ProductSuppliers] PS ON PS.ProductId = P.Id and PS.Active = 1
                                INNER JOIN [dbo].[Organisations] PSS ON PSS.Id = PS.SupplierId
                                GROUP BY P.Id
                        )
                        INSERT INTO @Products (ProductId, NoPurchaseOrders, OrderQty, NoShipments, ShipQty, ShipQtyInTransit, ShipNoCartons, ShipNoCartonsInTransit, AvailableSuppliers)
                        SELECT P.Id, ISNULL(PUR.NoPurchaseOrders,0), ISNULL(PUR.OrderQty,0), ISNULL(SHP.NoShipments,0), ISNULL(SHP.ShipQty,0), ISNULL(SHP.ShipQtyInTransit,0), ISNULL(SHP.ShipNoCartons,0), ISNULL(SHP.ShipNoCartonsInTransit,0), ISNULL(PSI.SupplierProductCount, 0)
                         FROM cte_product_ids PIs
                            INNER JOIN [dbo].[Products] P ON P.Id = PIs.Id
                            LEFT JOIN cte_po_totals PUR ON PUR.ProductId = P.Id
                            LEFT JOIN cte_ship_totals SHP ON SHP.ProductId = P.Id
                            LEFT JOIN cte_product_supplier_ids PSI ON PSI.ProductId = P.Id
                        WHERE P.CompanyId = @OrganisationId  AND P.Active = 1

                        SELECT P.*, 
                                ISNULL(Ps.NoPurchaseOrders,0) [ActiveOrders], ISNULL(Ps.ShipNoCartons, 0) [CartonQtyOnOrder], ISNULL(Ps.ShipNoCartonsInTransit, 0) [CartonQtyInTransit], 
                                (ISNULL(Ps.ShipNoCartons, 0) * P.UnitsPerPackage) [UnitsonOrder], (ISNULL(Ps.ShipNoCartonsInTransit, 0) * P.UnitsPerPackage) [UnitsInTransit],
                                CASE WHEN D.ThumbnailGenerated = 1 THEN D.ThumbnailUrl ELSE D.BlobUrl END [ThumbnailBlobUrl],
                                P.Tags
                        FROM @Products PS
                            INNER JOIN dbo.Products P ON P.Id = PS.ProductId
                            LEFT JOIN dbo.ProductDocuments PD ON PD.ProductId = P.Id AND PD.IsActive = 1 AND IsDefaultImage = 1
                            LEFT JOIN dbo.Documents D ON D.Id = PD.DocumentId AND D.Active = 1
                        ORDER BY {order_by_clause}
                        OFFSET (@PageNumber-1)*@PageSize ROWS
                        FETCH NEXT @PageSize ROWS ONLY

                        SELECT PS.ProductId, PS.SupplierId, PSS.Name [SupplierName], PS.SupplierReference, PS.Price, PS.OrderQuantityMinimum
                        FROM (   
                                SELECT PS.ProductId, (ISNULL(PS.ShipNoCartonsInTransit, 0) * P.UnitsPerPackage) [UnitsInTransit]
                                FROM @Products PS
                                    INNER JOIN dbo.Products P ON P.Id = PS.ProductId
                                ORDER BY {order_by_clause}
                                OFFSET (@PageNumber-1)*@PageSize ROWS
                                FETCH NEXT @PageSize ROWS ONLY
                            ) P
                            INNER JOIN [dbo].[ProductSuppliers] PS ON PS.ProductId = P.ProductId and PS.Active = 1
                            INNER JOIN [dbo].[Organisations] PSS ON PSS.Id = PS.SupplierId 
                        {where_supplier_clause.ToString()}

                        SELECT COUNT(*) FROM @Products;";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                                        new
                                        {
                                            @OrganisationId = request.OrganisationId,
                                            @search = BuildSearchParam(request.Search),
                                            @PageSize = request.Paging.PageSize,
                                            @PageNumber = request.Paging.PageNumber,
                                            @SupplierId = request.SupplierId.GetValueOrDefault()
                                        }))
                    {
                        List<ProductOrderInformation> products = (await multi.ReadAsync<ProductOrderInformation>()).ToList();

                        IEnumerable<ProductSupplierInformation> product_suppliers = await multi.ReadAsync<ProductSupplierInformation>();

                        int totalProductsCount = await multi.ReadFirstAsync<int>();

                        foreach (var prd in products)
                        {
                            prd.AvailableSuppliers = product_suppliers.Count(q => q.ProductId == prd.Id);
                            prd.Suppliers = new List<Core.Models.ProductSupplier>();

                            if (prd.AvailableSuppliers > 0) 
                                prd.Suppliers.AddRange(product_suppliers.Where(q => q.ProductId == prd.Id)
                                    .Select(s => 
                                        new Core.Models.ProductSupplier { 
                                            SupplierId = s.SupplierId, 
                                            OrderQuantityMinimum = s.OrderQuantityMinimum,
                                            SupplierReference = s.SupplierReference, 
                                            Price = s.Price,
                                            ProductId = s.ProductId,
                                            Supplier = new Core.Models.Organisation { Id = s.SupplierId, Name = s.SupplierName }
                                        }
                                    )
                                );
                        }

                        return new StaticPagedList<ProductResource>(products.Adapt<List<ProductResource>>().ToList(), request.Paging.PageNumber, request.Paging.PageSize, totalProductsCount);
                    }

                }
            }

            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }

            private string SortProductsQuery(string sortBy)
            {
                Dictionary<string, string> sortDictionary = new Dictionary<string, string>
                {
                   {"name.asc", "Name"},
                   {"name.desc", "Name desc"},
                   {"datecreated.asc", "CreationDateInternal"},
                   {"datecreated.desc", "CreationDateInternal desc"},
                   {"dateupdated.asc", "LastModifiedOnInternal"},
                   {"dateupdated.desc",  "LastModifiedOnInternal desc"},
                   {"availablesuppliers.desc", "AvailableSuppliers desc"},
                   {"unitsintransit.desc",  "UnitsInTransit desc"},
                   {"sku.asc",  "SKU"}
                };

                return sortDictionary.ContainsKey(sortBy ?? "")
                    ? sortDictionary[sortBy]
                    : "SKU";
            }
        }

    }
}
