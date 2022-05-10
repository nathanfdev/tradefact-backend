using Core.Common;
using Core.Enums;
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

namespace Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest
{
    public class GetQuotationRequestListQuery : IRequest<IPagedList<QuotationRequestInfo>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public QuotationSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetQuotationRequestListQueryHandler : IRequestHandler<GetQuotationRequestListQuery, IPagedList<QuotationRequestInfo>>
        {
            private readonly IDbConnection _connection;

            public GetQuotationRequestListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<QuotationRequestInfo>> Handle(GetQuotationRequestListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string organisation_type_switcher = request.OrganisationType == OrganisationTypeEnum.SHIPPER ? "P.ClientId" : "P.ProviderId";

                StringBuilder where_clause = new StringBuilder();
                if (request.Criteria.ShipmentMethod != null) where_clause.Append($" AND FM.ShipmentType = @ShipmentMethod");

                if (request.Criteria.Status != null)
                {
                    switch (request.Criteria.Status)
                    {
                        case QuotationStateEnum.PENDING:
                            where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.PENDING })");
                            break;
                        case QuotationStateEnum.READY:
                            where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.READY })");
                            break;
                        case QuotationStateEnum.ACCEPTED:
                            where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.ACCEPTED })");
                            break;
                        case QuotationStateEnum.EXPIRED:
                            where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.EXPIRED })");
                            break;
                        case QuotationStateEnum.REJECTED:
                            where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.REJECTED })");
                            break;

                        default:
                            where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.READY } OR QR.STATE = {(int)QuotationStateEnum.PENDING})");
                            break;
                    }

                } else
                {
                    where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.READY } OR QR.STATE = {(int)QuotationStateEnum.PENDING})");
                }

                //if (request.Criteria.ActiveOnly.GetValueOrDefault())
                //{
                //    where_clause.Append($" AND (QR.STATE = { (int)QuotationStateEnum.READY } OR QR.STATE = {(int)QuotationStateEnum.PENDING})");
                //}
                //else
                //{
                //    if (request.Criteria.Status != null) where_clause.Append($" AND QR.STATE = @status");
                //}

                string sQuery = @$"

                            DECLARE @quotation_requests TABLE (
                               QuotationRequestId UNIQUEIDENTIFIER NOT NULL,
                               FreightMovementId UNIQUEIDENTIFIER NOT NULL,
                               GoodsReady DATETIME2 NULL,
                               LastModifiedOnInternal DATETIME NULL,
                               PlaceofLoading VARCHAR(512),
                               PortofLoading VARCHAR(512),
                               PlaceofDischarge VARCHAR(512),
                               PortofDischarge VARCHAR(512),
                               IncoTerms int,
                               ShipmentType int,
                               [Name] VARCHAR(512),
                               LoadType int,
                               [Reference] VARCHAR(512),
                               [Partner] VARCHAR(512),
                               [Client] VARCHAR(512),
                               [State] int,
                               Submitted DATETIME2 NULL,
                               [Products] VARCHAR(512),
                               [Tags] VARCHAR(512),
                               [Revision] int,
                               [Search] VARCHAR(2)
                            )

                            ;WITH cte_quotation_requests AS (
                               SELECT QR.Id [QuotationRequestId], FM.[Id] [FreightMovementId], P.Id [PartnershipId], QR.[State]
                               FROM [dbo].[QuotationRequests] QR
                                INNER JOIN [dbo].[Partnerships] P ON P.Id = QR.PartnershipId
                                INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = QR.FreightMovementId
                               WHERE {organisation_type_switcher} = @OrganisationId{where_clause.ToString()}
                            ),
                            cte_revision AS (
                                SELECT QR.*, Q.LoadType FROM (
                                    SELECT QR.QuotationRequestId, MAX(ISNULL(Q.Revision,0)) [Revision]
                                    FROM cte_quotation_requests QR
                                    LEFT JOIN [dbo].[Quotations] Q ON Q.QuotationRequestId = QR.QuotationRequestId
                                    GROUP BY QR.QuotationRequestId
                                ) QR
                                LEFT JOIN [dbo].[Quotations] Q ON Q.QuotationRequestId = QR.QuotationRequestId AND Q.Revision = QR.Revision
                            ),
                            cte_quotation_requests_sku_filter AS (
                               SELECT CR.QuotationRequestId, CR.[FreightMovementId], CR.[PartnershipId], CR.[State], ISNULL(P.Name,'') + ISNULL(P.SKU, '') [Product]
                               FROM cte_quotation_requests CR
                                  LEFT JOIN [dbo].[CargoItems] CI ON CI.FreightMovementId = CR.FreightMovementId
                                  LEFT JOIN [dbo].[Products] P ON P.Id = CI.ProductId
                               GROUP BY CR.QuotationRequestId, CR.[FreightMovementId], CR.[PartnershipId], CR.[State], ISNULL(P.Name,'') + ISNULL(P.SKU, '')
                            ),
                            cte_quotation_requests_cargo_items AS (
                               SELECT CR.QuotationRequestId, CR.[FreightMovementId], CR.[PartnershipId], CR.[State], STRING_AGG(CR.Product, ' ') AS Products
                               FROM cte_quotation_requests_sku_filter CR
                               GROUP BY CR.QuotationRequestId, CR.[FreightMovementId], CR.[PartnershipId], CR.[State]
                            ),
                            cte_raw as (
                            SELECT QR.QuotationRequestId, FM.GoodsReady, Q.LastModifiedOnInternal, FM.Id [FreightMovementId],
                                ISNULL(FM.PlaceOfLoadingMultiple ,'(' + AL.CountryCode + ') - ' + AL.Name) [PlaceofLoading], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], 
                                '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
                                FM.IncoTerms, FM.ShipmentType, FM.Name, ISNULL(R.LoadType, FM.LoadType) [LoadType], FM.Reference, PR.Name [Partner], CL.Name [Client], SU.[Name] [Supplier], QR.[State], Q.Submitted, QR.Products, FM.Tags, R.Revision
                            FROM cte_quotation_requests_cargo_items QR
                                INNER JOIN [dbo].[QuotationRequests] Q ON Q.Id = QR.QuotationRequestId
                                LEFT JOIN cte_revision R ON R.QuotationRequestId = QR.QuotationRequestId
                                INNER JOIN [dbo].[Partnerships] P ON P.Id = QR.PartnershipId
                                LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = QR.FreightMovementId
                                LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                LEFT JOIN [dbo].[Organisations] SU ON SU.Id = AL.OrganisationId
                                LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = FM.PortOfLoadingId
                                LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = FM.PortOfDischargeId
                            ),
                            cte_orders AS (
                                SELECT QuotationRequestId, STRING_AGG(PurchaseOrderNumber + ' ' + Reference, ' ') [PurchaseOrderSearch]
                                FROM (
                                    SELECT DISTINCT CR.QuotationRequestId, PO.PurchaseOrderNumber, PO.Reference
                                    FROM cte_quotation_requests_sku_filter CR
                                    JOIN FreightMovementItems FMI ON FMI.FreightMovementId = CR.FreightMovementId
                                    JOIN PurchaseOrders PO ON PO.Id = FMI.PurchaseOrderId
                                ) a
                                GROUP BY QuotationRequestId
                            ),
                            cte_search as (
                            SELECT CS.*, ISNULL(CS.PlaceofLoading, '') + ISNULL(CS.PortofLoading, '') + ISNULL(CS.PlaceofDischarge, '') + ISNULL(CS.PortofDischarge, '') + ISNULL(CS.Reference, '') + ISNULL(CS.Name, '') + 
                                ISNULL(CS.Supplier, '') + ISNULL(CS.Products, '') + ISNULL(CS.Tags, '') + ' ' + ISNULL(CO.PurchaseOrderSearch, '') [Search]
                              FROM cte_raw CS
                              LEFT OUTER JOIN cte_orders CO ON CO.QuotationRequestId = CS.QuotationRequestId
                            )
                            INSERT INTO @quotation_requests (
                                QuotationRequestId, FreightMovementId, GoodsReady, LastModifiedOnInternal, PlaceofLoading, PortofLoading, PlaceofDischarge,
                                PortofDischarge, IncoTerms, ShipmentType, [Name], LoadType, [Reference], [Partner],
                                [Client], [State], Submitted, [Products], [Tags], [Revision], [Search]
                            )
                            SELECT  QuotationRequestId, FreightMovementId, GoodsReady, LastModifiedOnInternal, PlaceofLoading, PortofLoading, PlaceofDischarge,
                                    PortofDischarge, IncoTerms, ShipmentType, [Name], LoadType, [Reference], [Partner],
                                    [Client], [State], Submitted, NULL [Products], [Tags], [Revision], NULL [Search]
                             FROM cte_search CS
                            WHERE CS.Search like @search OR @search is NULL
                            ORDER BY LastModifiedOnInternal desc

                            SELECT QuotationRequestId [Id], FreightMovementId, GoodsReady, LastModifiedOnInternal, PlaceofLoading, PortofLoading, PlaceofDischarge,
                                PortofDischarge, IncoTerms, ShipmentType, [Name], LoadType, [Reference], [Partner],
                                [Client], [State], Submitted, [Products], [Tags], [Revision], [Search]
                            FROM @quotation_requests QR
                            ORDER BY QR.LastModifiedOnInternal desc
                            OFFSET (@PageNumber-1)*@PageSize ROWS
                            FETCH NEXT @PageSize ROWS ONLY

                            SELECT QRS.*,
                                    PO.PurchaseOrderNumber [PurchaseOrderNumber], PO.Reference [PurchaseOrderReference], 
                                    O.Id [SupplierId], O.Name [SupplierName], 
                                    '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading] FROM (
                                SELECT QuotationRequestId [Id], FM.Id [FreightMovementId], 
                                    PSL.[PurchaseOrderId] [PurchaseOrderId],
                                    ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady) [GoodsReady], PSL.[Description] [ScheduleName]
                                FROM (  SELECT QuotationRequestId, FreightMovementId 
                                        FROM @quotation_requests QR 
                                        ORDER BY QR.LastModifiedOnInternal desc
                                        OFFSET (@PageNumber-1)*@PageSize ROWS
                                        FETCH NEXT @PageSize ROWS ONLY
                                        ) QR
                                    INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = QR.FreightMovementId
                                    INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                    LEFT JOIN [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                GROUP BY QuotationRequestId, FM.Id, PSL.[PurchaseOrderId], ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady), PSL.[Description] 
                            ) QRS
                                LEFT JOIN [dbo].[PurchaseOrders] PO ON PO.Id = QRS.PurchaseOrderId
                                LEFT JOIN [dbo].[Organisations] O ON O.Id = PO.SupplierId
                                LEFT JOIN [dbo].[Addresses] AL ON AL.Id = QRS.PlaceOfLoadingId
                            ORDER BY QRS.Id, PlaceOfLoadingId

                            SELECT DISTINCT QR.QuotationRequestId, PO.Id [PurchaseOrderId], PO.PurchaseOrderNumber
                            FROM @quotation_requests QR
                            JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = QR.FreightMovementId
                            JOIN [dbo].[PurchaseOrders] PO ON PO.Id = FMI.PurchaseOrderId

                            SELECT COUNT(QuotationRequestId) [Quotes] FROM @quotation_requests";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @OrganisationId = request.OrganisationId,
                        @status = request.Criteria.Status.GetValueOrDefault(),
                        @ShipmentMethod = request.Criteria.ShipmentMethod.GetValueOrDefault(),
                        @search = BuildSearchParam(request.Criteria.Search),
                        @PageSize = request.Criteria.CombineRelatedQuotes ? int.MaxValue : request.Paging.PageSize,
                        @PageNumber = request.Criteria.CombineRelatedQuotes ? 1 : request.Paging.PageNumber,
                    }, commandTimeout: 60))
                    {
                        List<QuotationRequestInfo> query_result_requests = (await multi.ReadAsync<QuotationRequestInfo>()).ToList();

                        List<QuotationRequestScheduleInfo> query_result_schedules = (await multi.ReadAsync<QuotationRequestScheduleInfo>()).ToList();

                        List<QuotationPurchaseOrderInfo> query_po_numbers = (await multi.ReadAsync<QuotationPurchaseOrderInfo>()).ToList();

                        int totalQuotesCount = await multi.ReadFirstAsync<int>();

                        foreach (QuotationRequestInfo qr in query_result_requests)
                        {
                            qr.Schedules = query_result_schedules.Where(q => q.Id == qr.Id).ToList();
                            if (qr.Schedules?.Count > 0)
                            {
                                qr.GoodsReadyMin = qr.Schedules.Min(s => s.GoodsReady);
                                qr.GoodsReadyMax = qr.Schedules.Max(s => s.GoodsReady);

                                qr.GoodsReadyDates = qr.Schedules.OrderBy(o => o.GoodsReady).Select(s => s.GoodsReady).Distinct().ToArray();
                                qr.NoOfGoodsReadyDates = qr.GoodsReadyDates.Count();
                            }

                            qr.PurchaseOrderInfo = query_po_numbers
                                .Where(q => q.QuotationRequestId == qr.Id)
                                .ToList();
                        }

                        if (request.Criteria.CombineRelatedQuotes)
                        {
                            var groupedFMQuotationList = query_result_requests
                                .GroupBy(u => u.FreightMovementId)
                                .Select(grp => grp.ToList())
                                .ToList();

                            var quotation_result = new List<QuotationRequestInfo>();

                            foreach (List<QuotationRequestInfo> fmList in groupedFMQuotationList)
                            {
                                QuotationRequestInfo firstItem = fmList.First();

                                if (fmList.Count > 1)
                                {
                                    List<PartnerQuoteInfo> distinctPartners = fmList.Select(fm => new PartnerQuoteInfo { QuotationId = fm.Id, State = fm.State, Name = fm.Partner }).ToList();
                                    firstItem.Partners = distinctPartners;
                                    firstItem.Partner = null;
                                }

                                quotation_result.Add(firstItem);
                            }

                            IPagedList<QuotationRequestInfo> paginated_quotation_result = await quotation_result.ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                            return paginated_quotation_result;
                        }

                        return new StaticPagedList<QuotationRequestInfo>(query_result_requests.Adapt<List<QuotationRequestInfo>>(), request.Paging.PageNumber, request.Paging.PageSize, totalQuotesCount);
                    }
                }
            }

            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }
        }
    }
}
