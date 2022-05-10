using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models.Shipment;

namespace Tradefact.Application.Shipments.Queries.GetShipmentInfo
{
    public class GetShipmentPackingListQuery : IRequest<ShipmentPackingListInfo>
    {
        public Guid ShipmentId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid PlaceOfLoadingId { get; set; }
        public Guid ShipperId { get; set; }

        public class GetShipmentPackingListQueryHandler : IRequestHandler<GetShipmentPackingListQuery, ShipmentPackingListInfo>
        {
            private readonly IDbConnection _connection;
            private readonly IMediator _mediator;

            public GetShipmentPackingListQueryHandler(IDbConnection connection, IMediator mediator)
            {
                _connection = connection;
                this._mediator = mediator;
            }

            public async Task<ShipmentPackingListInfo> Handle(GetShipmentPackingListQuery request, System.Threading.CancellationToken cancellationToken)
            {

                ShipmentPackingListInfo ship_pack_list = new ShipmentPackingListInfo();
                ship_pack_list.Shipment = await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = request.ShipmentId, ClientId = request.ShipperId });

                string sQuery = @$";WITH cte_points AS (
                                        SELECT S.Id [ShipmentId], PO.Id [PurchaseOrderId], PSL.PlaceOfLoadingId, PO.SupplierId, O.Name, PO.PurchaseOrderNumber, PO.Reference,  FM.PlaceOfDispatchId, S.PortOfLoadingId, S.PortOfDischargeId 
                                            FROM [dbo].[Shipments] S
                                            INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                                            INNER JOIN [dbo].[CargoItems] C ON C.FreightMovementId = FM.Id
                                            LEFT JOIN [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.Id = C.PurchaseOrderItemScheduleLineId
                                            LEFT JOIN [dbo].[PurchaseOrders] PO ON PO.Id = PSL.PurchaseOrderId
                                            LEFT JOIN [dbo].[Organisations] O ON O.Id = PO.SupplierId
                                        WHERE S.Id = @PackingListKey_ShipmentId
                                        GROUP BY S.Id, PO.SupplierId, O.Name, PO.Id, PO.PurchaseOrderNumber, PO.Reference, PSL.PlaceOfLoadingId, FM.PlaceOfDispatchId, S.PortOfLoadingId, S.PortOfDischargeId
                                    )
                                    , cte_legs AS (
                                        SELECT 1 [Seq], 0 [IsPort], 'Place Of Loading' [Description], P.PlaceOfLoadingId, AD.Name [Name], AD.[AddressLine1] [AddressLine1], AD.[AddressLine2] [AddressLine2], 
                                                AD.[AddressLine3] [AddressLine3], AD.[AddressLine4] [AddressLine4], AD.[City] [City], AD.[County] [County], AD.[PostalCode] [Postcode], P.PortOfLoadingId, P.PortOfDischargeId
                                        FROM cte_points P
                                            INNER JOIN [dbo].[Addresses] AD ON AD.Id = P.PlaceOfLoadingId
                                        UNION
                                        SELECT  2 [Seq], 1 [IsPort], 'Port Of Loading' [Description], P.PlaceOfLoadingId, '['+L.LocCode+'] '+ L.Name [Name] , C.Name [AddressLine1], NULL [AddressLine2], 
                                                NULL [AddressLine3], NULL [AddressLine4], NULL [City], NULL [County], NULL [Postcode], P.PortOfLoadingId, P.PortOfDischargeId
                                        FROM cte_points P
                                            INNER JOIN [dbo].[Locations] L ON L.LocCode = P.PortOfLoadingId
                                            INNER JOIN [dbo].[Countries] C ON C.Code2 = L.CountryCode
                                        UNION
                                        SELECT  3 [Seq], 1 [IsPort], 'Port Of Discharge' [Description], P.PlaceOfLoadingId, '['+L.LocCode+'] '+ L.Name [Name] , C.Name [AddressLine1], NULL [AddressLine2], 
                                                NULL [AddressLine3], NULL [AddressLine4], NULL [City], NULL [County], NULL [Postcode], P.PortOfLoadingId, P.PortOfDischargeId
                                        FROM cte_points P
                                            INNER JOIN [dbo].[Locations] L ON L.LocCode = P.PortOfDischargeId
                                            INNER JOIN [dbo].[Countries] C ON C.Code2 = L.CountryCode
                                        UNION
                                        SELECT  4 [Seq], 0 [IsPort], 'Place Of Despatch' [Description], P.PlaceOfLoadingId, AD.Name [Name], AD.[AddressLine1] [AddressLine1], AD.[AddressLine2] [AddressLine2], 
                                                AD.[AddressLine3] [AddressLine3], AD.[AddressLine4] [AddressLine4], AD.[City] [City], AD.[County] [County], AD.[PostalCode] [Postcode], P.PortOfLoadingId, P.PortOfDischargeId
                                        FROM cte_points P
                                            INNER JOIN [dbo].[Addresses] AD ON AD.Id = P.PlaceOfDispatchId
                                    )
                                    SELECT * FROM [cte_legs] ORDER BY [Seq]


                                    SELECT  CI.ShipmentId, CI.PurchaseOrderId, PO.PurchaseOrderNumber, CI.PurchaseOrderItemId, 
                                            CI.Product_SKU,  P.Id [ProductId], P.Barcode,
                                            ISNULL(CI.Product_Description, P.[Name]) Product_Description , CI.QtyCommitted
                                     FROM (
                                        SELECT  S.Id [ShipmentId], 
                                            C.PurchaseOrderId [PurchaseOrderId], 
                                            C.PurchaseOrderItemId,
                                            C.ProductId, C.SKU [Product_SKU], 
                                            C.ProductDescriptionOverride [Product_Description], 
                                            SUM (ISNULL(C.Qty,0)) [QtyCommitted]
                                        FROM [dbo].[Shipments] S
                                            INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                                            INNER JOIN [dbo].[CargoItems] C ON C.FreightMovementId = FM.Id
                                            INNER JOIN [dbo].[Products] P ON P.Id = c.ProductId
                                            LEFT JOIN [dbo].[PurchaseOrderItems] POI ON POI.PurchaseOrderId = C.PurchaseOrderId AND POI.Id = C.PurchaseOrderItemId
                                        WHERE S.Id = @PackingListKey_ShipmentId --AND PO.SupplierId = @PackingListKey_SupplierId and PSL.PlaceOfLoadingId = @PackingListKey_PlaceOfLoadingId 
                                        GROUP BY  S.Id, C.PurchaseOrderId, 
                                                C.PurchaseOrderItemId,
                                                C.ProductId, 
                                                C.SKU, 
                                                C.ProductDescriptionOverride
                                    ) CI
                                    LEFT JOIN [dbo].[Products] P ON P.Id = CI.ProductId
                                    LEFT JOIN [dbo].[PurchaseOrders] PO ON PO.Id = CI.PurchaseOrderId

                ";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @PackingListKey_ShipmentId = request.ShipmentId,
                        @PackingListKey_SupplierId = request.SupplierId,
                        @PackingListKey_PlaceOfLoadingId = request.PlaceOfLoadingId,
                    }, commandTimeout: 60))
                    {
                        ship_pack_list.Legs = (await multi.ReadAsync<ShipmentLegInfo>()).ToList();

                        List<ShipmentProductInfo> consignment = (await multi.ReadAsync<ShipmentProductInfo>()).ToList();

                        ship_pack_list.Products = consignment.GroupBy(k => new {k.ShipmentId, k.Product_SKU, k.Product_Description })
                              .Select(kg =>
                                    new ShipmentProductInfo {
                                        ShipmentId = kg.Key.ShipmentId,
                                        Product_SKU = kg.Key.Product_SKU,
                                        Product_Description = kg.Key.Product_Description,
                                        QtyCommitted = kg.Sum(w => w.QtyCommitted),
                                        PurchaseOrders = kg.Select(s=>s.PurchaseOrderNumber).Distinct().ToList()
                                    }).ToList();

                    }
                }
                return ship_pack_list;
            }
        }
    }
}
