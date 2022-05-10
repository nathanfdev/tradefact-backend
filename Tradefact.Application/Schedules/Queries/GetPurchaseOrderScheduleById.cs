using Core.Common;
using Core.Enums;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Schedules.Model;
using X.PagedList;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderScheduleByIdQuery : IRequest<PurchaseOrderScheduleLineResource>
    {
        public Guid ScheduleLineId { get; set; }
        public bool IncludeItems { get; set; } = false;

        public PagedResultParameters Paging { get; set; }

        public class GetPurchaseOrderScheduleByIdQueryHandler : IRequestHandler<GetPurchaseOrderScheduleByIdQuery, PurchaseOrderScheduleLineResource>
        {
            private readonly IDbConnection _connection;
            private readonly ILogger<GetPurchaseOrderScheduleByIdQueryHandler> _logger;

            // Using DI to inject infrastructure persistence Repositories
            public GetPurchaseOrderScheduleByIdQueryHandler(IDbConnection connection, ILogger<GetPurchaseOrderScheduleByIdQueryHandler> logger)
            {

                _connection = connection ?? throw new ArgumentNullException(nameof(connection));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<PurchaseOrderScheduleLineResource> Handle(GetPurchaseOrderScheduleByIdQuery request, System.Threading.CancellationToken cancellationToken)
            {

                StringBuilder sqlBuilder = new StringBuilder();

                sqlBuilder.AppendLine(@$"
                    ;WITH cte_schedule_lines AS (
                        SELECT  PIS.PurchaseOrderId, PIS.Id [ScheduleLineId], PIS.Description [Name], ISNULL(MAX(PIS.ConfirmedGoodsReadyDate),MAX(PIS.RequestedGoodsReadyDate)) [GoodsReady],
                                PIS.PlaceOfLoadingId, CountryofLoadingCode, COUNT(PIS.Id) [Lines], SUM(ScheduleLineCommittedQuantity) [Items], SUM(ScheduleLineCommittedQuantity * PI.OrderPriceUnit) [LineValue],
                                CASE WHEN (
                                    SELECT COUNT (*) 
                                    FROM [dbo].FreightMovementItems FMI
                                       INNER JOIN [dbo].QuotationRequests QR ON QR.FreightMovementId = FMI.FreightMovementId
                                    WHERE FMI.PurchaseOrderItemScheduleLineId =  @schedule_line_id) > 0 THEN 1 ELSE 0 END [IsLocked]
                        FROM [dbo].[PurchaseOrderItemScheduleLines] PIS
                            INNER JOIN [dbo].[PurchaseOrderItems] PI ON PI.PurchaseOrderId = PIS.PurchaseOrderId and PI.Id = PIS.PurchaseOrderItemId
                        WHERE PIS.Id = @schedule_line_id
                        GROUP BY PIS.PurchaseOrderId, PIS.Id, PIS.PlaceOfLoadingId, PIS.Description, CountryofLoadingCode
                    )
                    SELECT  SL.PurchaseOrderId, SL.[Name], PO.PurchaseOrderNumber, PO.Reference [Reference], SL.ScheduleLineId, SL.GoodsReady, SL.Lines, SL.Items, SL.[LineValue], S.Currency [CurrencyId], SL.PlaceOfLoadingId,
                            C.Code2 [CountryofLoadingCode], C.Name [Country],
                            AD.Name [PlaceOfLoading], AD.AddressLine1 [PlaceOfLoading_Address1], AD.AddressLine2 [PlaceOfLoading_Address2],  AD.AddressLine3 [PlaceOfLoading_Address3], AD.AddressLine4 [PlaceOfLoading_Address4], AD.City [PlaceOfLoading_City],
                            S.Name [Supplier], SL.IsLocked
                    FROM cte_schedule_lines SL
                        INNER JOIN [dbo].PurchaseOrders PO ON PO.Id = SL.PurchaseOrderId
                        INNER JOIN [dbo].Addresses AD ON AD.Id = SL.PlaceOfLoadingId
                        INNER JOIN [dbo].Countries C ON C.Code2 = AD.CountryCode
                        INNER JOIN [dbo].Organisations S ON S.Id = PO.SupplierId");

                if (request.IncludeItems)
                {
                    sqlBuilder.AppendLine(@$"
                    SELECT  PIS.PurchaseOrderId ,PIS.PurchaseOrderItemId, PIS.Id [PurchaseOrderItemScheduleLineId], PIS.RequestedDeliveryDate, PIS.ConfirmedDeliveryDate,	
                            PIS.ScheduleLineOrderQuantity, PIS.ScheduleLineCommittedQuantity, PIS.OrderQuantityUnit,	
                            PIS.ScheduleLineOrderWeight, PIS.PlaceOfLoadingId, PIS.CountryofLoadingCode,
                            PIS.CreatedByUser, PIS.CreationDateInternal, PIS.LastChangeUser, PIS.LastModifiedOnInternal,	
                            S.Name,
                            P.Id [ProductId],
                            ISNULL(PI.PurchaseOrderItemText, P.Name) [Product_Name],
                            ISNULL(PI.PurchaseOrderItemText, P.Description) [Product_Description],
                            P.Dimensions_Height [Product_Dimensions_Height],
                            P.Dimensions_Length [Product_Dimensions_Length],
                            P.Dimensions_Scale [Product_Dimensions_Scale],
                            P.Dimensions_Width [Product_Dimensions_Width],
                            P.GoodsType [Product_GoodsType],
                            P.HsCode [Product_HsCode],
                            P.Nickname [Product_Nickname],
                            P.Packing [Product_Packing],
                            P.SKU [Product_SKU],
                            P.CompanyId [Product_CompanyId],
                            P.HazardClass [Product_HazardClass],
                            P.HazardDocumentId [Product_HazardDocumentId],
                            P.HazardNotes [Product_HazardNotes],
                            P.MagneticFieldContained [Product_MagneticFieldContained],
                            P.Reference [Product_Reference],
                            P.HazardDescription [Product_HazardDescription],
                            P.UnitsPerPackage [Product_UnitsPerPackage],
                            P.HazardousContents [Product_HazardousContents],
                            P.Rotatable [Product_Rotatable],
                            P.Stackable [Product_Stackable],
                            P.Dimensions_Weight [Product_Dimensions_Weight],
                            P.Dimensions_weightMeasurement [Product_Dimensions_weightMeasurement],
                            P.LithiumBatteryPacking [Product_LithiumBatteryPacking],
                            PIS.ScheduleLineCommittedQuantity * PI.OrderPriceUnit [LineValue], S.Currency [CurrencyId]
                    FROM [dbo].[PurchaseOrderItemScheduleLines] PIS
                        INNER JOIN [dbo].[PurchaseOrderItems] PI ON PI.PurchaseOrderId = PIS.PurchaseOrderId AND PI.Id = PIS.PurchaseOrderItemId
                        INNER JOIN [dbo].PurchaseOrders PO ON PO.Id = PI.PurchaseOrderId
                        INNER JOIN [dbo].Organisations S ON S.Id = PO.SupplierId
                        INNER JOIN [dbo].Products P ON P.Id = PI.ProductId
                    WHERE PIS.Id = @schedule_line_id");
                }

                string sQuery = sqlBuilder.ToString();
                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    if (!request.IncludeItems)
                    {
                        PurchaseOrderScheduleLineResourceDTO result = await conn.QuerySingleOrDefaultAsync<PurchaseOrderScheduleLineResourceDTO>(sQuery, new
                        {
                            @schedule_line_id = request.ScheduleLineId
                        }, commandTimeout: 60);
                        return result.Adapt<PurchaseOrderScheduleLineResource>();
                    }

                    using (var multi = conn.QueryMultiple(sQuery, new
                    {
                        @schedule_line_id = request.ScheduleLineId
                    }, commandTimeout: 60))
                    {
                        PurchaseOrderScheduleLineResourceDTO sc = multi.Read<PurchaseOrderScheduleLineResourceDTO>().FirstOrDefault();
                        List<PurchaseOrderScheduleLineItemResourceDTO> sc_items = multi.Read<PurchaseOrderScheduleLineItemResourceDTO>().ToList();

                        PurchaseOrderScheduleLineResource schedule = sc.Adapt<PurchaseOrderScheduleLineResource>();
                        schedule.Schedule = await sc_items.Adapt<List<PurchaseOrderScheduleLineItemResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                        return schedule;
                    }

                }
            }
        }
    }
}
