using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderItemsByIdQuery : IRequest<List<PurchaseOrderItemResource>>
    {
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }

        public List<Guid> PurchaseOrderItemIds { get; set; }

        public class GetPurchaseOrderItemsByIdQueryHandler : IRequestHandler<GetPurchaseOrderItemsByIdQuery, List<PurchaseOrderItemResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderItemsByIdQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<List<PurchaseOrderItemResource>> Handle(GetPurchaseOrderItemsByIdQuery request, System.Threading.CancellationToken cancellationToken)
            {

                string sQuery = @$"
SELECT  PI.PurchaseOrderId, PI.Id, PI.ProductId,
        CASE WHEN P.Id IS NULL THEN 1 ELSE 0 END [Product_IsVariant],
        PI.OrderQuantity, PI.OrderPriceUnit, PI.OrderQuantityUnit, PI.SupplierReference,
        ISNULL(P.Description,PV.Description) [Product_Description],
        ISNULL(P.Dimensions_Height,PV.Dimensions_Height) [Product_Dimensions_Height],
        ISNULL(P.Dimensions_Length,PV.Dimensions_Length) [Product_Dimensions_Length],
        ISNULL(P.Dimensions_Scale,PV.Dimensions_Scale) [Product_Dimensions_Scale],
        ISNULL(P.Dimensions_Width,PV.Dimensions_Width) [Product_Dimensions_Width],
        ISNULL(P.GoodsType,PV.GoodsType) [Product_GoodsType],
        ISNULL(P.HsCode,PV.HsCode) [Product_HsCode],
        ISNULL(P.Name,PV.Name) [Product_Name],
        ISNULL(P.Nickname,PV.Nickname) [Product_Nickname],
        ISNULL(P.Packing,PV.Packing) [Product_Packing],
        ISNULL(P.SKU,PV.SKU) [Product_SKU],
        ISNULL(P.CompanyId,PVP.CompanyId) [Product_CompanyId],
        ISNULL(P.HazardClass,PV.HazardClass) [Product_HazardClass],
        ISNULL(P.HazardDocumentId,PV.HazardDocumentId) [Product_HazardDocumentId],
        ISNULL(P.HazardNotes,PV.HazardNotes) [Product_HazardNotes],
        ISNULL(P.MagneticFieldContained,PV.MagneticFieldContained) [Product_MagneticFieldContained],
        ISNULL(P.Reference,PV.Reference) [Product_Reference],
        ISNULL(P.HazardDescription,PV.HazardDescription) [Product_HazardDescription],
        ISNULL(P.UnitsPerPackage,PV.UnitsPerPackage) [Product_UnitsPerPackage],
        ISNULL(P.HazardousContents,PV.HazardousContents) [Product_HazardousContents],
        ISNULL(P.Rotatable,PVP.Rotatable) [Product_Rotatable],
        ISNULL(P.Stackable,PVP.Stackable) [Product_Stackable],
        ISNULL(P.Dimensions_Weight,PV.Dimensions_Weight) [Product_Dimensions_Weight],
        ISNULL(P.Dimensions_weightMeasurement,PV.Dimensions_weightMeasurement) [Product_Dimensions_weightMeasurement],
        ISNULL(P.LithiumBatteryPacking,PVP.LithiumBatteryPacking) [Product_LithiumBatteryPacking]
FROM [dbo].[PurchaseOrderItems] PI
    LEFT JOIN [dbo].PurchaseOrders PO ON PO.id = PI.PurchaseOrderId     
    LEFT JOIN [dbo].Products P ON P.id = PI.ProductId     
    LEFT JOIN [dbo].ProductVariants PV ON PV.id = PI.ProductId 
    LEFT JOIN [dbo].Products PVP ON PVP.id = PV.ProductId
WHERE PI.PurchaseOrderId = @PurchaseOrderId AND PI.Id IN ('{String.Join("','", request.PurchaseOrderItemIds.Select(s => s.ToString()).ToArray())}')";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    IEnumerable<PurchaseOrderItemDTO> poItems = await conn.QueryAsync<PurchaseOrderItemDTO>(sQuery, new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId
                    });
                    return poItems.Adapt<List<PurchaseOrderItemResource>>();
                }

            }
        }
    }
}
