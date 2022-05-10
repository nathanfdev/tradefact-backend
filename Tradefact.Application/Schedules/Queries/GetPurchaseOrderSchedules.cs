using Core.Common;
using Core.Enums;
using Core.Models.Criteria;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Schedules.Model;
using X.PagedList;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderSchedulesQuery : IRequest<IPagedList<PurchaseOrderScheduleLineResource>>
    {
        public Guid OrganisationId { get; set; }
        public SchedulesSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetPurchaseOrderSchedulesQueryHandler : IRequestHandler<GetPurchaseOrderSchedulesQuery, IPagedList<PurchaseOrderScheduleLineResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ILogger<GetPurchaseOrderSchedulesQueryHandler> _logger;

            // Using DI to inject infrastructure persistence Repositories
            public GetPurchaseOrderSchedulesQueryHandler(IDbConnection connection, ILogger<GetPurchaseOrderSchedulesQueryHandler> logger)
            {

                _connection = connection ?? throw new ArgumentNullException(nameof(connection));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<IPagedList<PurchaseOrderScheduleLineResource>> Handle(GetPurchaseOrderSchedulesQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder where_clause = new StringBuilder();

                if (request.Criteria.Schedules != null && request.Criteria.Schedules.Count > 0) where_clause.Append($" AND PIS.Id IN ('{String.Join("','", request.Criteria.Schedules.ToArray())}')");
                if (!String.IsNullOrEmpty(request.Criteria.CountryCode)) where_clause.Append($" AND PIS.CountryofLoadingCode = '{request.Criteria.CountryCode}'");

                string sQuery = @$";WITH cte_schedule_lines AS (
                                        SELECT  PIS.PurchaseOrderId, PIS.Id [ScheduleLineId], PIS.Description [Name], ISNULL(MAX(PIS.ConfirmedGoodsReadyDate),MAX(PIS.RequestedGoodsReadyDate)) [GoodsReady],
                                                PIS.PlaceOfLoadingId, CountryofLoadingCode, COUNT(PIS.Id) [Lines], SUM(ScheduleLineCommittedQuantity) [Items], SUM(ScheduleLineCommittedQuantity * PI.OrderPriceUnit) [LineValue],
                                                PIS.CreationDateInternal, CASE WHEN COUNT(DISTINCT QR.Id) > 0 THEN 1 ELSE 0 END [IsLocked]
                                        FROM [dbo].[PurchaseOrderItemScheduleLines] PIS 
                                            INNER JOIN [dbo].[PurchaseOrderItems] PI ON PI.PurchaseOrderId = PIS.PurchaseOrderId and PI.Id = PIS.PurchaseOrderItemId
                                            INNER JOIN [dbo].PurchaseOrders PO ON PO.Id = PIS.PurchaseOrderId
                                            LEFT OUTER JOIN [dbo].FreightMovementItems FMI on FMI.PurchaseOrderItemScheduleLineId = PIS.Id
                                            LEFT OUTER JOIN [dbo].QuotationRequests QR ON QR.FreightMovementId = FMI.FreightMovementId
                                        WHERE PO.CompanyId = @organisation_id AND PIS.Active = 1 {where_clause.ToString()}
                                        GROUP BY PIS.PurchaseOrderId, PIS.Id, PIS.Description, PIS.PlaceOfLoadingId, CountryofLoadingCode, PIS.CreationDateInternal
                                    )
                                    SELECT  SL.PurchaseOrderId, SL.[Name], PO.PurchaseOrderNumber, PO.Reference [Reference], SL.ScheduleLineId, SL.GoodsReady, SL.Lines, SL.Items, SL.[LineValue], S.Currency [CurrencyId],
                                            C.Code2 [CountryofLoadingCode], C.Name [Country],
                                            SL.PlaceOfLoadingId, 
                                            AD.Name [PlaceOfLoading], AD.AddressLine1 [PlaceOfLoading_Address1], AD.AddressLine2 [PlaceOfLoading_Address2],  AD.AddressLine3 [PlaceOfLoading_Address3], AD.AddressLine4 [PlaceOfLoading_Address4], AD.City [[PlaceOfLoading_City]]
                                            S.Name [Supplier], SL.IsLocked
                                    FROM cte_schedule_lines SL
                                        INNER JOIN [dbo].PurchaseOrders PO ON PO.Id = SL.PurchaseOrderId
                                        INNER JOIN [dbo].Addresses AD ON AD.Id = SL.PlaceOfLoadingId
                                        INNER JOIN [dbo].Countries C ON C.Code2 = AD.CountryCode
                                        INNER JOIN [dbo].Organisations S ON S.Id = PO.SupplierId
                                        WHERE PO.PurchaseOrderNumber like @search OR SL.[Name] like @search OR @search is NULL
                                        ORDER BY SL.CreationDateInternal desc";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    var query_result = await conn.QueryAsync<PurchaseOrderScheduleLineResourceDTO>(sQuery, new
                    {
                        @organisation_id = request.OrganisationId,
                        @search = BuildSearchParam(request.Criteria.Search)
                    }, commandTimeout: 60);
                    IPagedList<PurchaseOrderScheduleLineResource> purchase_order_schedules = await query_result.Adapt<List<PurchaseOrderScheduleLineResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                    return purchase_order_schedules;
                }
            }

            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }
        }
    }
}
