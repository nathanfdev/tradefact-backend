using Dapper;
using Mapster;
using MediatR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderItemsQuery : IRequest<List<PurchaseOrderItemResource>>
    {
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }

        public class GetPurchaseOrderItemsQueryHandler : IRequestHandler<GetPurchaseOrderItemsQuery, List<PurchaseOrderItemResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderItemsQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<List<PurchaseOrderItemResource>> Handle(GetPurchaseOrderItemsQuery request, System.Threading.CancellationToken cancellationToken)
            {

                string sQuery = @$";WITH cte_schedule_items AS (
                                        SELECT PurchaseOrderItemId, SUM(ScheduleLineCommittedQuantity)[ScheduleLineCommittedQuantity] FROM dbo.PurchaseOrderItemScheduleLines WHERE Active = 1 GROUP BY PurchaseOrderItemId
                                    )
                                    SELECT  PI.PurchaseOrderId, PI.Id, PI.ProductId,
                                        ISNULL(PV.ProductId, P.Id) [MasterProductId],
                                        CASE WHEN P.Id IS NULL THEN 1 ELSE 0 END [IsProductVariant],
                                        PI.OrderQuantity, ISNULL(SI.ScheduleLineCommittedQuantity, 0)[ScheduleLineCommittedQuantity], PI.OrderQuantity - ISNULL(SI.ScheduleLineCommittedQuantity, 0) [RemainingQuantity], PI.OrderPriceUnit, PI.OrderQuantityUnit, PI.SupplierReference,
                                        ISNULL(PI.PurchaseOrderItemText, ISNULL(P.Name,PV.Name)) [Product_Name],
                                        ISNULL(PI.PurchaseOrderItemText, ISNULL(P.Description,PV.Description)) [Product_Description],
                                        P.Tags [Product_Tags],
                                        ISNULL(P.Dimensions_Height,PV.Dimensions_Height) [Product_Dimensions_Height],
                                        ISNULL(P.Dimensions_Length,PV.Dimensions_Length) [Product_Dimensions_Length],
                                        ISNULL(P.Dimensions_Scale,PV.Dimensions_Scale) [Product_Dimensions_Scale],
                                        ISNULL(P.Dimensions_Width,PV.Dimensions_Width) [Product_Dimensions_Width],
                                        ISNULL(P.GoodsType,PV.GoodsType) [Product_GoodsType],
                                        ISNULL(P.HsCode,PV.HsCode) [Product_HsCode],
                                        ISNULL(P.Nickname,PV.Nickname) [Product_Nickname],
                                        ISNULL(P.Packing,PV.Packing) [Product_Packing],
                                        ISNULL(PI.SKU, ISNULL(P.SKU,PV.SKU)) [Product_SKU],
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
                                        ISNULL(P.LithiumBatteryPacking,PVP.LithiumBatteryPacking) [Product_LithiumBatteryPacking],
                                        CASE WHEN PI.PurchaseOrderItemText IS NULL THEN 0 ELSE 1 END [ProductDescriptionOverride],
                                        CASE
                                        WHEN D.ThumbnailGenerated = CAST(1 AS bit) THEN D.ThumbnailUrl
                                        ELSE D.BlobUrl END AS [Product_ThumbnailBlobUrl]
                                            FROM [dbo].[PurchaseOrderItems] PI
                                                LEFT JOIN [dbo].PurchaseOrders PO ON PO.id = PI.PurchaseOrderId     
                                                LEFT JOIN [dbo].Products P ON P.id = PI.ProductId     
                                                LEFT JOIN [dbo].ProductVariants PV ON PV.id = PI.ProductId 
                                                LEFT JOIN [dbo].Products PVP ON PVP.id = PV.ProductId
                                                LEFT JOIN cte_schedule_items SI ON SI.PurchaseOrderItemId = PI.Id
                                                LEFT JOIN [dbo].[ProductDocuments] PD ON P.Id = PD.ProductId AND PD.IsActive = 1 AND PD.IsDefaultImage = 1
                                                LEFT JOIN [dbo].[Documents] D ON D.Id = PD.DocumentId  AND D.Active = 1
                                            WHERE PI.PurchaseOrderId = @PurchaseOrderId
                                            AND PI.Active = 1
                                            ORDER BY PI.LastModifiedOnInternal DESC;

                                        SELECT SL.*, S.Currency [CurrencyId], C.Code2 [CountryofLoadingCode], C.Name [Country],
                                            AD.Name [PlaceOfLoading], AD.AddressLine1 [PlaceOfLoading_Address1], AD.AddressLine2 [PlaceOfLoading_Address2],
                                            AD.AddressLine3 [PlaceOfLoading_Address3], AD.AddressLine4 [PlaceOfLoading_Address4], AD.City [PlaceOfLoading_City],
                                            S.Name [Supplier]
                                            FROM (
                                                SELECT PIS.PurchaseOrderId, PIS.PurchaseOrderItemId, PIS.Id [ScheduleId], PIS.[Description], PIS.ConfirmedGoodsReadyDate, 
                                                    PlaceOfLoadingId, CountryofLoadingCode, COUNT(PIS.Id) [Lines], SUM(PIS.ScheduleLineCommittedQuantity) [ItemQty],
                                                    SUM(PIS.ScheduleLineCommittedQuantity * PI.OrderPriceUnit) [LineValue]
                                                FROM [dbo].[PurchaseOrderItemScheduleLines] PIS
                                                    INNER JOIN [dbo].[PurchaseOrderItems] PI ON PI.PurchaseOrderId = PIS.PurchaseOrderId and PI.Id = PIS.PurchaseOrderItemId
                                                    WHERE PIS.PurchaseOrderId = @PurchaseOrderId AND PIS.Active = 1
                                                GROUP BY PIS.PurchaseOrderId, PIS.PurchaseOrderItemId, PIS.Id, PIS.ConfirmedGoodsReadyDate, PIS.PlaceOfLoadingId, PIS.Description, CountryofLoadingCode
                                        ) SL
                                            INNER JOIN [dbo].PurchaseOrders PO ON PO.Id = SL.PurchaseOrderId
                                            INNER JOIN [dbo].Addresses AD ON AD.Id = SL.PlaceOfLoadingId
                                            INNER JOIN [dbo].Countries C ON C.Code2 = AD.CountryCode
                                            INNER JOIN [dbo].Organisations S ON S.Id = PO.SupplierId

                                        SELECT  FM.*, S.[Status], S.Booked, S.InTransit, s.Delivered, CASE WHEN S.Id IS NULL THEN 1 ELSE 0 END [QuotationRequested],
                                                ISNULL(S.PortOfLoadingId, F.PortOfLoadingId) [PortOfLoadingId] , '('+PL.CountryCode+') - '+ PL.Name [PortOfLoading],
                                                ISNULL(S.PortOfDischargeId, F.PortOfDischargeId) [PortOfDischargeId], '('+PD.CountryCode+') - '+ PD.Name [PortOfDischarge],
                                                F.ShipmentType, F.IncoTerms, F.LoadType, S.ETA, S.ETD, ISNULL(S.Tags, F.Tags) [Tags]
                                            FROM (
                                            SELECT CI.PurchaseOrderId [PurchaseOrderId], CI.PurchaseOrderItemId, CI.FreightMovementId
                                            FROM [dbo].[CargoItems] CI 
                                            WHERE CI.PurchaseOrderId = @PurchaseOrderId
                                            GROUP BY CI.PurchaseOrderId, CI.PurchaseOrderItemId, CI.FreightMovementId
                                        ) FM
                                            INNER JOIN [dbo].[FreightMovements] F ON F.Id = FM.FreightMovementId
                                            LEFT JOIN [dbo].[Shipments] S ON S.FreightMovementId = FM.FreightMovementId
                                            LEFT JOIN [dbo].Locations PL ON PL.LocCode = CONVERT(varchar(36), ISNULL(S.PortOfLoadingId, F.PortOfLoadingId))
                                            LEFT JOIN [dbo].Locations PD ON PD.LocCode = CONVERT(varchar(36), ISNULL(S.PortOfDischargeId, F.PortOfDischargeId))
";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId

                    }, commandTimeout: 60))
                    {
                        List<PurchaseOrderItemDTO> query_result_po_items = (await multi.ReadAsync<PurchaseOrderItemDTO>()).ToList();

                        List<PurchaseOrderItemScheduleInformationDTO> query_result_schedules = (await multi.ReadAsync<PurchaseOrderItemScheduleInformationDTO>()).ToList();

                        List<PurchaseOrderItemShipmentInformationDTO> query_result_shipments = (await multi.ReadAsync<PurchaseOrderItemShipmentInformationDTO>()).ToList();


                        foreach (PurchaseOrderItemDTO po_item in query_result_po_items)
                        {
                            po_item.Schedules = query_result_schedules.Where(q => q.PurchaseOrderId == po_item.PurchaseOrderId && q.PurchaseOrderItemId == po_item.Id).ToList();
                            po_item.Shipments = query_result_shipments.Where(q => q.PurchaseOrderId == po_item.PurchaseOrderId && q.PurchaseOrderItemId == po_item.Id).ToList();
                        }

                        return query_result_po_items.Adapt<List<PurchaseOrderItemResource>>();
                    }

                }

            }
        }
    }
}
