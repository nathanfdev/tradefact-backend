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
using System.Linq;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetAllPurchaseOrdersQuery : IRequest<IPagedList<PurchaseOrderResource>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public PurchaseOrderSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, IPagedList<PurchaseOrderResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetAllPurchaseOrdersQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            private string BuildWhereClause(GetAllPurchaseOrdersQuery request)
            {
                StringBuilder where_clause = new StringBuilder();

                where_clause.Append($"P.Active = {(request.Criteria.IsDeleted ? 0 : 1 )}");

                if (request.Criteria.GetSalesOrders)
                {
                    where_clause.Append(" AND P.SupplierId = @OrganisationId");
                }
                else if (request.OrganisationType == OrganisationTypeEnum.SHIPPER)
                {
                    where_clause.Append(" AND P.CompanyId = @OrganisationId");
                    if (request.Criteria.SupplierId.HasValue)
                    {
                        where_clause.Append($" AND P.SupplierId = '{request.Criteria.SupplierId}'");
                    }
                }
                else
                {
                    where_clause.Append(" AND P.SupplierId = @OrganisationId");
                }

                if (request.Criteria.CreatedByMe) where_clause.Append($" AND P.CreatedByUser = '{_userService.Email}'");

                if (request.Criteria.ShipmentMethod != null) where_clause.Append($" AND P.ShipmentType = @ShipmentMethod");

                List<int> statusList = new List<int>();


                if (request.Criteria.Completed) {
                    statusList.Add(100);
                } else
                {
                    if (request.Criteria.DraftOnly == true)
                    {
                        statusList.Add(0);
                    } else
                    {
                        if (request.Criteria.Status.HasValue) {
                            statusList.Add(request.Criteria.Status.GetValueOrDefault());
                        } else
                        {
                            statusList.AddRange(new int[]{10, 20, 30, 40, 50, 100 });
                            if (request.Criteria.IsDeleted)
                            {
                                statusList.Add(0);
                            }
                        }
                    }
                }
                if (statusList.Any())
                {
                    where_clause.Append($" AND P.Status IN ({ String.Join(", ", statusList.Select(x => x.ToString()).Distinct())})");
                }
                return where_clause.ToString();
            }

            private string BuildPOSelect(GetAllPurchaseOrdersQuery request)
            {
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.AppendLine(@$"
                        ;WITH cte_purchaseorders AS (
                            SELECT P.[Id] [PurchaseOrderId], P.LastModifiedOnInternal
                            FROM [dbo].[PurchaseOrders] P
                            WHERE {this.BuildWhereClause(request)}  
                        )");

                if (String.IsNullOrEmpty(request.Criteria.Search))
                {
                    queryBuilder.AppendLine(@"
                        INSERT INTO @AllPOs (
                            PurchaseOrderId, LastModifiedOnInternal
                        )
                        SELECT PurchaseOrderId, LastModifiedOnInternal
                         FROM cte_purchaseorders PO
                        ORDER BY PO.LastModifiedOnInternal desc
                    ");

                } else
                {
                    //  Need to build query which only returns orders matching search criteria
                    queryBuilder.AppendLine(@$"
                    ,cte_po_sku_filter AS (
                        SELECT PO.PurchaseOrderId, ISNULL(PI.PurchaseOrderItemText, ISNULL(P.Name, '') + ' ' + ISNULL(PI.SKU, ISNULL(P.SKU, '')) + ' ' + ISNULL(P.Nickname,'')) [Product]
                        FROM cte_purchaseorders PO
                        LEFT JOIN [dbo].[PurchaseOrderItems] PI ON PI.PurchaseOrderId = PO.PurchaseOrderId
                        LEFT JOIN [dbo].[Products] P ON P.Id = PI.ProductId
                        GROUP BY PO.PurchaseOrderId, ISNULL(PI.PurchaseOrderItemText, ISNULL(P.Name, '') + ' ' + ISNULL(PI.SKU, ISNULL(P.SKU, '')) + ' ' + ISNULL(P.Nickname,''))
                    )");
                    queryBuilder.AppendLine(@$"
                    ,cte_po_items AS (
                        SELECT CR.PurchaseOrderId, STRING_AGG(cast(CR.Product as NVARCHAR(MAX)), ' ') AS Products
                        FROM cte_po_sku_filter CR
                        GROUP BY CR.PurchaseOrderId
                    ),                    
                    cte_raw as (
                        SELECT  P.[PurchaseOrderId],
                                PO.PurchaseOrderNumber, 
                                PO.Reference,
                                SU.Name [Supplier],
                                CL.Name [Client],
                                PO.Tags,
                                '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading],
                                AL.Name [PlaceofLoading_Name],
                                PL.LocCode [PortofLoadingId], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading],

                                '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDispatch],
                                AD.Name [PlaceofDispatch_Name],
                                PD.LocCode [PortofDischargeId], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
                                PI.[Products] [Products]
                        FROM cte_purchaseorders P
                            LEFT JOIN cte_po_items PI ON PI.PurchaseOrderId = P.PurchaseOrderId
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
                    )");

                    queryBuilder.AppendLine(@$"
                        ,cte_search as (
                            SELECT *, ISNULL(CS.Client, '') + ' ' + ISNULL(CS.Supplier, '') + ' ' + 
                            REPLACE(ISNULL(CS.Tags, ''), ',', ' ') + 
                            ISNULL(CS.PlaceofLoading, '') + ISNULL(CS.PortofLoading, '') + ISNULL(CS.PlaceofLoading_Name, '') +
                            ISNULL(CS.PlaceofDispatch, '') + ISNULL(CS.PortofDischarge, '') + ISNULL(CS.PlaceofDispatch_Name, '') + 
                            ISNULL(CS.Reference, '') + ' ' + ISNULL(CS.PurchaseOrderNumber, '') + ' ' + ISNULL(CS.Products, '') + ' ' + ISNULL(CS.Tags, '') [Search]
                            FROM cte_raw CS
                        )");

                    queryBuilder.AppendLine(@"
                        INSERT INTO @AllPOs (
                            PurchaseOrderId, LastModifiedOnInternal
                        )
                        SELECT CO.PurchaseOrderId, CO.LastModifiedOnInternal
                        FROM cte_search CS
                            INNER JOIN cte_purchaseorders CO ON CO.PurchaseOrderId = CS.PurchaseOrderId
                        WHERE Search like @search OR @search is NULL"
                    );

                }

                return queryBuilder.ToString();
            }

            public async Task<IPagedList<PurchaseOrderResource>> Handle(GetAllPurchaseOrdersQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string organisation_type_switcher = request.OrganisationType == OrganisationTypeEnum.SHIPPER ? "P.ClientId" : "P.ProviderId";

                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.AppendLine(@$"DECLARE @POs TABLE
                        (   
                            PurchaseOrderId UNIQUEIDENTIFIER NOT NULL,
                            LastModifiedOnInternal DATETIME NULL,
                            Attachments INT DEFAULT 0
                        )

                        DECLARE @AllPOs TABLE
                        (   
                            PurchaseOrderId UNIQUEIDENTIFIER NOT NULL,
                            LastModifiedOnInternal DATETIME NULL
                        )");

                queryBuilder.AppendLine(this.BuildPOSelect(request));

                queryBuilder.AppendLine(@"

                        INSERT INTO @POs (
                            PurchaseOrderId, LastModifiedOnInternal
                        )
                        SELECT PurchaseOrderId, LastModifiedOnInternal
                        FROM @AllPOs PO
                        ORDER BY PO.LastModifiedOnInternal desc
                        OFFSET (@PageNumber-1)*@PageSize ROWS
                        FETCH NEXT @PageSize ROWS ONLY

                        ;WITH cte_purchaseorder_attachment_counts AS (
                            SELECT PO.PurchaseOrderId, COUNT(D.Id) [Attachments] FROM @POs PO
                                INNER JOIN PurchaseOrderDocuments PD ON PD.PurchaseOrderId = PO.PurchaseOrderId
                                INNER JOIN Documents D ON D.Id = PD.DocumentId
                            WHERE D.Active = 1
                            GROUP BY PO.PurchaseOrderId
                        )
                        UPDATE POs SET [Attachments] = POc.Attachments
                         FROM @POs POs
                            INNER JOIN cte_purchaseorder_attachment_counts POc ON POc.PurchaseOrderId = POs.PurchaseOrderId


                        SELECT POs.[PurchaseOrderId], PO.PurchaseOrderNumber, PO.Reference, PO.CompanyId, SU.Name [Supplier], CL.Name [Customer], PO.SupplierId, 
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
                            PO.PreShipment, PO.PreShipmentDate, PO.Cancelled, PO.CancelledDate, PO.Deleted, PO.DeletionDate,

                            PO.LoadType, PO.IncoTerms, PO.ShipmentType, SU.Name [Provider], CL.Name [Client], PO.CreationDateInternal, PO.LastModifiedOnInternal, PO.CreatedByUser, NULL [Products],
                            ISNULL(PO.TaxRate,0) [TaxRate], 
                            ISNULL(PO.BaseCurrency_NetAmount,0) [BaseCurrency_NetAmount], ISNULL(PO.BaseCurrency_TaxAmount,0) [BaseCurrency_TaxAmount], ISNULL(PO.BaseCurrency_TotalAmount,0) [BaseCurrency_TotalAmount], 
                            ISNULL(PO.Total_NetAmount,0) [Total_NetAmount], ISNULL(PO.Total_TaxAmount,0) [Total_TaxAmount], ISNULL(PO.Total_TotalAmount,0) [Total_TotalAmount], PO.NumberOfItems, POs.Attachments, PO.IsLocked, PO.[Active] [Active]
                         FROM @POs POs
                            INNER JOIN [dbo].[PurchaseOrders] PO ON PO.Id = POs.PurchaseOrderId 
                            LEFT JOIN [dbo].[Organisations] SU ON SU.Id = PO.SupplierId
                            LEFT JOIN [dbo].[Organisations] CL ON CL.Id = PO.CompanyId
                            LEFT JOIN [dbo].[Addresses] AL ON AL.Id = PO.PlaceOfLoadingId
                            LEFT JOIN [dbo].[Countries] ALC ON ALC.Code2 = AL.CountryCode
                            LEFT JOIN [dbo].[Addresses] AD ON AD.Id = PO.PlaceOfDispatchId
                            LEFT JOIN [dbo].[Countries] ADC ON ADC.Code2 = AD.CountryCode
                            LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = PO.PortOfLoadingId
                            LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = PO.PortOfDischargeId   
                            LEFT JOIN [dbo].[AspNetUsers] AU ON AU.Email = PO.CreatedByUser
                        ORDER BY PO.LastModifiedOnInternal desc

                        SELECT P.PurchaseOrderId [PurchaseOrderId], PS.Id [ScheduleId], PS.ConfirmedGoodsReadyDate, PS.[Description]
                        FROM @POs P 
                                INNER JOIN [dbo].PurchaseOrderItemScheduleLines PS ON PS.PurchaseOrderId = P.PurchaseOrderId
                        GROUP BY P.PurchaseOrderId, PS.Id, PS.ConfirmedGoodsReadyDate, PS.[Description]

                        SELECT  FM.*, S.[Status], S.Booked, S.InTransit, S.Delivered, 
                                S.PortOfLoadingId, '('+PL.CountryCode+') - '+ PL.Name [PortOfLoading],
                                S.PortOfDischargeId, '('+PD.CountryCode+') - '+ PD.Name [PortOfDischarge],
                                FMS.ShipmentType, S.IncoTerms, FMS.LoadType, S.ETA, S.ETD, S.Tags, FMS.Name, QR.State [QuoteState]
                         FROM (
                            SELECT P.PurchaseOrderId [PurchaseOrderId], FMI.FreightMovementId
                            FROM @POs P 
                                    INNER JOIN [dbo].FreightMovementItems FMI ON FMI.PurchaseOrderId = P.PurchaseOrderId
                            GROUP BY P.PurchaseOrderId, FMI.FreightMovementId
                        ) FM
                            LEFT JOIN [dbo].[Shipments] S ON S.FreightMovementId = FM.FreightMovementId
                            LEFT JOIN [dbo].[QuotationRequests] QR ON QR.FreightMovementId = FM.FreightMovementId
                            LEFT JOIN [dbo].FreightMovements FMS ON FMS.Id = FM.FreightMovementId
                            LEFT JOIN [dbo].Locations PL ON PL.LocCode = CONVERT(varchar(36),S.PortOfLoadingId)
                            LEFT JOIN [dbo].Locations PD ON PD.LocCode = CONVERT(varchar(36),S.PortOfDischargeId)
                            WHERE (S.Booked = 1 AND QR.State = 2) OR S.Booked IS NULL AND (QR.State = 0 OR QR.State = 1)
                            ORDER BY FMS.CreationDateInternal desc

                        SELECT COUNT(PurchaseOrderId) FROM @AllPOs");

                string sQuery = queryBuilder.ToString();

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @OrganisationId = request.OrganisationId,
                        @search = BuildSearchParam(request.Criteria.Search),
                        @PageSize = request.Paging.PageSize,
                        @PageNumber = request.Paging.PageNumber,
                    }, commandTimeout: 60))
                    {
                        List<PurchaseOrderDTO> query_result_pos = (await multi.ReadAsync<PurchaseOrderDTO>()).ToList();

                        List<PurchaseOrderScheduleInformationDTO> query_result_schedules = (await multi.ReadAsync<PurchaseOrderScheduleInformationDTO>()).ToList();

                        List<PurchaseOrderShipmentInformationDTO> query_result_shipments = (await multi.ReadAsync<PurchaseOrderShipmentInformationDTO>()).ToList();

                        int totalProductsCount = await multi.ReadFirstAsync<int>();

                        foreach (PurchaseOrderDTO po in query_result_pos)
                        {
                            po.IsOwned = (po.CompanyId == request.OrganisationId);

                            po.Schedules = query_result_schedules.Where(Q => Q.PurchaseOrderId == po.PurchaseOrderId).ToList();
                            po.Shipments = query_result_shipments.Where(Q => Q.PurchaseOrderId == po.PurchaseOrderId).ToList();
                        }

                        return new StaticPagedList<PurchaseOrderResource>(query_result_pos.Adapt<List<PurchaseOrderResource>>(), request.Paging.PageNumber, request.Paging.PageSize, totalProductsCount);
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
