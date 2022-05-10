using Core.Common;
using Core.Enums;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using X.PagedList;
using Mapster;
using Core.Models;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetAllPurchaseOrdersNoProductsQuery : IRequest<IPagedList<PurchaseOrderResource>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public PurchaseOrderSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetAllPurchaseOrdersNoProductsQueryHandler : IRequestHandler<GetAllPurchaseOrdersNoProductsQuery, IPagedList<PurchaseOrderResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetAllPurchaseOrdersNoProductsQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<IPagedList<PurchaseOrderResource>> Handle(GetAllPurchaseOrdersNoProductsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string organisation_type_switcher = request.OrganisationType == OrganisationTypeEnum.SHIPPER ? "P.ClientId" : "P.ProviderId";

                StringBuilder where_clause = new StringBuilder();

                if (request.OrganisationType == OrganisationTypeEnum.SHIPPER)
                {
                    where_clause.Append("P.CompanyId = @OrganisationId");
                    if (request.Criteria.SupplierId.HasValue)
                    {
                        where_clause.Append($" AND P.SupplierId = '{request.Criteria.SupplierId}'");
                    }
                }
                else
                {
                    where_clause.Append("P.SupplierId = @OrganisationId");
                }

                if (request.Criteria.CreatedByMe) where_clause.Append($" AND P.CreatedByUser = '{_userService.Email}'");

                if (request.Criteria.ShipmentMethod != null) where_clause.Append($" AND P.ShipmentType = @ShipmentMethod");
                if (request.Criteria.Completed)
                {
                    where_clause.Append($" AND P.Status = 100");
                }
                else if (request.Criteria.HasException)
                {
                    where_clause.Append($" AND P.Status = 60");
                }
                else if (request.Criteria.DraftOnly == true)
                {
                    where_clause.Append(" AND P.Status = 0");
                }
                else if (request.Criteria.AllNoDrafts == true)
                {
                    if (request.Criteria.Status.HasValue) where_clause.Append($" AND (P.Active=1 AND P.Status = {request.Criteria.Status.GetValueOrDefault()})");
                    else where_clause.Append(" AND NOT (P.Status = 0)");
                }
                else if (request.Criteria.ActiveOnly == true)
                {
                    if (request.Criteria.Status.HasValue) where_clause.Append($" AND (P.Active=1 AND P.Status = {request.Criteria.Status.GetValueOrDefault()})");
                    else where_clause.Append(" AND (P.Status = 10 OR P.Status = 20 OR P.Status = 30 OR P.Status = 40 OR P.Status = 50 OR P.Status = 100)");
                }
                else if (request.Criteria.ActiveOnly == false)
                {
                    if (request.Criteria.Status.HasValue) where_clause.Append($" AND (P.Active=1 AND P.Status = {request.Criteria.Status.GetValueOrDefault()})");
                    else where_clause.Append(" AND (P.Status = 70)");
                }
                else
                {
                   if (request.Criteria.Status.HasValue) where_clause.Append($" AND (P.Active=1 AND P.Status = {request.Criteria.Status.GetValueOrDefault()})");
                }

                StringBuilder sqlBuilder = new StringBuilder();

                sqlBuilder.AppendLine(@$"
                    ;WITH cte_purchaseorders AS (
                        SELECT P.[Id] [PurchaseOrderId], P.CompanyId, P.SupplierId
                        FROM [dbo].[PurchaseOrders] P
                        WHERE P.[Active] = 1 AND {where_clause.ToString()}  
                    )");


                sqlBuilder.AppendLine(@$"                  
                    ,cte_raw as (
                        SELECT P.[PurchaseOrderId], PO.PurchaseOrderNumber, PO.Reference, PO.CompanyId, SU.Name [Supplier], CL.Name [Customer], PO.SupplierId, 
                            PL.LocCode [PortofLoadingId], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading],
                            PD.LocCode [PortofDischargeId], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
                            PO.PlaceOfLoadingId, 
                            '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading],
                            AL.Name [PlaceofLoading_Name],
                            AL.AddressLine1 [PlaceofLoading_AddressLine1],
                            AL.AddressLine2 [PlaceofLoading_AddressLine2],
                            AL.AddressLine3 [PlaceofLoading_AddressLine3],
                            AL.AddressLine4 [PlaceofLoading_AddressLine4],
                            AL.City [PlaceofLoading_City],
                            AL.PostalCode [PlaceofLoading_PostalCode],
                            AL.CountryCode [PlaceofLoading_Country_Code],
                            ALC.Name [PlaceofLoading_Country_Name],
                            PO.PlaceOfDispatchId, 
                            '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDispatch],
                            AD.Name [PlaceofDispatch_Name],
                            AD.AddressLine1 [PlaceofDispatch_AddressLine1],
                            AD.AddressLine2 [PlaceofDispatch_AddressLine2],
                            AD.AddressLine3 [PlaceofDispatch_AddressLine3],
                            AD.AddressLine4 [PlaceofDispatch_AddressLine4],
                            AD.City [PlaceofDispatch_City],
                            AD.PostalCode [PlaceofDispatch_PostalCode],
                            AD.CountryCode [PlaceofDispatch_Country_Code],
                            ADC.Name [PlaceofDispatch_Country_Name],
                            PO.GoodsReadyDate,
                            PO.PurchaseOrderDate,
                            PO.TargetDeliveryDate,
                            PO.CurrencyId,
                            PO.RejectedReason,
                            PO.OrderRejected,
                            PO.DateOfIssue,
                            PO.Tags, PO.Status,
                            AU.FullName [CreatedByName],

                            PO.Submitted, PO.SubmittedDate, PO.Accepted, PO.AcceptedDate, PO.Rejected, PO.RejectedDate, 
                            PO.InProduction, PO.InProductionDate, PO.Shipping, PO.ShippedDate, PO.Completed, PO.CompletedDate, 
                            PO.PreShipment, PO.PreShipmentDate, PO.Cancelled, PO.CancelledDate,

                            PO.LoadType, PO.IncoTerms, PO.ShipmentType, SU.Name [Provider], CL.Name [Client], PO.CreationDateInternal, PO.LastModifiedOnInternal, PO.CreatedByUser,
                            ISNULL(PO.TaxRate,0) [TaxRate], 
                            ISNULL(PO.BaseCurrency_NetAmount,0) [BaseCurrency_NetAmount], ISNULL(PO.BaseCurrency_TaxAmount,0) [BaseCurrency_TaxAmount], ISNULL(PO.BaseCurrency_TotalAmount,0) [BaseCurrency_TotalAmount], 
                            ISNULL(PO.Total_NetAmount,0) [Total_NetAmount], ISNULL(PO.Total_TaxAmount,0) [Total_TaxAmount], ISNULL(PO.Total_TotalAmount,0) [Total_TotalAmount], PO.NumberOfItems
                        FROM cte_purchaseorders P
                            INNER JOIN [dbo].[PurchaseOrders] PO ON PO.Id = P.PurchaseOrderId
                            LEFT JOIN [dbo].[Organisations] SU ON SU.Id = PO.SupplierId
                            LEFT JOIN [dbo].[Organisations] CL ON CL.Id = PO.CompanyId
                            LEFT JOIN [dbo].[Addresses] AL ON AL.Id = PO.PlaceOfLoadingId
                            LEFT JOIN [dbo].[Countries] ALC ON ALC.Code2 = AL.CountryCode
                            LEFT JOIN [dbo].[Addresses] AD ON AD.Id = PO.PlaceOfDispatchId
                            LEFT JOIN [dbo].[Countries] ADC ON ADC.Code2 = AD.CountryCode
                            LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = PO.PortOfLoadingId
                            LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = PO.PortOfDischargeId
                            LEFT JOIN [dbo].[AspNetUsers] AU ON AU.Email = PO.CreatedByUser
                    ),");
                
                    if (!String.IsNullOrEmpty(request.Criteria.Search))
                    {
                        sqlBuilder.AppendLine(@$"
                        cte_search as (
                            SELECT *, ISNULL(CS.Client, '') + ' ' + ISNULL(CS.Supplier, '') + ' ' + 
                            REPLACE(ISNULL(CS.Tags, ''), ',', ' ') + 
                            ISNULL(CS.PlaceofLoading, '') + ISNULL(CS.PortofLoading, '') + ISNULL(CS.PlaceofLoading_Name, '') +
                            ISNULL(CS.PlaceofDispatch, '') + ISNULL(CS.PortofDischarge, '') + ISNULL(CS.PlaceofDispatch_Name, '') + 
                            ISNULL(CS.Reference, '') + ' ' + ISNULL(CS.PurchaseOrderNumber, '') + ' ' + ISNULL(CS.Tags, '') [Search]
                            FROM cte_raw CS
                        )");
                    }
                    else
                    {
                        sqlBuilder.AppendLine(@$"cte_search as (
                            SELECT *, '' [Search]
                            FROM cte_raw CS
                        )");
                    }

                sqlBuilder.AppendLine(@$"
                    SELECT *, {(int)request.OrganisationType} [OrganisationType]
                        FROM cte_search PO
                    WHERE PO.Search like @search OR @search is NULL
                    ORDER BY PO.LastModifiedOnInternal desc"
                );

                string sQuery = sqlBuilder.ToString();

                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    IEnumerable<PurchaseOrderDTO> query_result = await conn.QueryAsync<PurchaseOrderDTO>(sQuery, new
                    {
                        request.OrganisationId,
                        @search = BuildSearchParam(request.Criteria.Search)
                    }, commandTimeout: 60);
                    IPagedList<PurchaseOrderResource> purchase_orders = await query_result.Adapt<List<PurchaseOrderResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                    return purchase_orders;
                }
            }


            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }
        }
    }
}
