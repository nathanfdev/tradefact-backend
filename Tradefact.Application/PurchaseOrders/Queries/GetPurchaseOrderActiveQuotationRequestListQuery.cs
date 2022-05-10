using Core.Common;
using Core.Enums;
using Core.Models;
using Core.Models.Criteria;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderActiveQuotationRequestListQuery : IRequest<IPagedList<QuotationRequestInfo>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrderShipmentSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetPurchaseOrderActiveQuotationRequestListQueryHandler : IRequestHandler<GetPurchaseOrderActiveQuotationRequestListQuery, IPagedList<QuotationRequestInfo>>
        {
            private readonly IDbConnection _connection;
            public GetPurchaseOrderActiveQuotationRequestListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<QuotationRequestInfo>> Handle(GetPurchaseOrderActiveQuotationRequestListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder queryBuilder = new StringBuilder();
                if (request.Criteria.ShipmentMethod.HasValue) queryBuilder.Append($" AND FM.ShipmentType = {(int)request.Criteria.ShipmentMethod}");
                if (request.Criteria.ScheduleId != null && request.Criteria.ScheduleId.Count() > 0) queryBuilder.Append($" AND CAST(FMI.PurchaseOrderItemScheduleLineId AS VARCHAR(MAX)) IN ('{ String.Join("','", request.Criteria.ScheduleId)}')");

                string sQuery = @$";DECLARE @QRs TABLE
                                    (   
                                        QuotationRequestId UNIQUEIDENTIFIER NOT NULL,
                                        Revision INT DEFAULT 0
                                    )

                                    INSERT INTO @QRs (QuotationRequestId, Revision)
                                    SELECT QR.Id [QuotationRequestId], MAX(ISNULL(Q.Revision,0)) [Revision]
                                        FROM [dbo].[QuotationRequests] QR
                                            LEFT JOIN [dbo].[Quotations] Q ON Q.QuotationRequestId = QR.Id
                                            INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = QR.FreightMovementId
                                            INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                            WHERE FMI.PurchaseOrderId = @PurchaseOrderId AND (QR.STATE = 1 OR QR.STATE = 0) 
                                            {queryBuilder.ToString()}
                                    GROUP BY QR.Id

                                    SELECT QR.QuotationRequestId [Id], FM.GoodsReady, Q.LastModifiedOnInternal, 
                                        ISNULL(FM.PlaceOfLoadingMultiple ,'(' + AL.CountryCode + ') - ' + AL.Name) [PlaceofLoading], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], 
                                        '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
                                        FM.IncoTerms, FM.ShipmentType, FM.Name, FM.LoadType, FM.Reference, PR.Name [Partner], CL.Name [Client],  Q.[State],  Q.Submitted, '' [Products], FM.Tags, QR.Revision
                                    FROM @QRs QR
                                        INNER JOIN [dbo].[QuotationRequests] Q ON Q.Id = QR.QuotationRequestId
                                        INNER JOIN [dbo].[Partnerships] P ON P.Id = Q.PartnershipId
                                        LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                        LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                        INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = Q.FreightMovementId
                                        LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                        LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                        LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = FM.PortOfLoadingId
                                        LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = FM.PortOfDischargeId
                                    ORDER BY Q.LastModifiedOnInternal desc

                                    SELECT QRS.QuotationRequestId [Id], QRS.*, '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading] FROM (
                                        SELECT QR.[QuotationRequestId], FM.Id [FreightMovementId], ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady) [GoodsReady], PSL.[Description] [ScheduleName]
                                        FROM @QRs QR
                                            INNER JOIN [dbo].[QuotationRequests] Q ON Q.Id = QR.QuotationRequestId
                                            INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = Q.FreightMovementId
                                            INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                            LEFT JOIN [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                        GROUP BY QR.[QuotationRequestId], FM.Id, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady), PSL.[Description] 
                                    ) QRS
                                        LEFT JOIN [dbo].[Addresses] AL ON AL.Id = QRS.PlaceOfLoadingId
                                    ORDER BY QRS.[QuotationRequestId], PlaceOfLoadingId";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId
                    }, commandTimeout: 60))
                    {
                        List<QuotationRequestInfo> query_result_quotations = (await multi.ReadAsync<QuotationRequestInfo>()).ToList();

                        List<QuotationRequestScheduleInfo> query_result_quotation_schedules = (await multi.ReadAsync<QuotationRequestScheduleInfo>()).ToList();

                        foreach (QuotationRequestInfo qr in query_result_quotations)
                        {
                            qr.Schedules = query_result_quotation_schedules.Where(q => q.Id == qr.Id).ToList();
                            if (qr.Schedules.Count > 0)
                            {
                                qr.GoodsReadyMin = qr.Schedules.Min(s => s.GoodsReady);
                                qr.GoodsReadyMax = qr.Schedules.Max(s => s.GoodsReady);

                                qr.GoodsReadyDates = qr.Schedules.OrderBy(o => o.GoodsReady).Select(s => s.GoodsReady).Distinct().ToArray();
                                qr.NoOfGoodsReadyDates = qr.GoodsReadyDates.Count();
                            } else
                            {
                                qr.GoodsReadyMin = qr.GoodsReady;
                                qr.GoodsReadyMax = qr.GoodsReady;

                                qr.GoodsReadyDates = new List<DateTime> {qr.GoodsReady}.ToArray();
                                qr.NoOfGoodsReadyDates = qr.GoodsReadyDates.Count();
                            }
                        }
                        int count = query_result_quotations.Count;
                        count = (count == 0) ? int.MaxValue : count;
                        return new StaticPagedList<QuotationRequestInfo>(query_result_quotations.Adapt<List<QuotationRequestInfo>>(), request.Paging.PageNumber, count, count);
                    }
                }
            }
        }
    }
}
