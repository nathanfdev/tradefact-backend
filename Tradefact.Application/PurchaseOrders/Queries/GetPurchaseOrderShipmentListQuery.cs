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
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models;
using Tradefact.Application.Models.Shipment;
using Tradefact.Data;
using X.PagedList;



namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderShipmentListQuery : IRequest<IPagedList<ShipmentInfo>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrderShipmentSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetPurchaseOrderShipmentListQueryHandler : IRequestHandler<GetPurchaseOrderShipmentListQuery, IPagedList<ShipmentInfo>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderShipmentListQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<IPagedList<ShipmentInfo>> Handle(GetPurchaseOrderShipmentListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder queryBuilder = new StringBuilder();
                if (request.Criteria.ShipmentMethod.HasValue) queryBuilder.Append($" AND S.ShipmentType = {(int)request.Criteria.ShipmentMethod}");
                if (request.Criteria.ScheduleId != null && request.Criteria.ScheduleId.Count() > 0) queryBuilder.Append($" AND PurchaseOrderItemScheduleLineId IN ('{ String.Join("','", request.Criteria.ScheduleId.Select(s=>s.ToString().Trim()))}')");

                string sQuery = @$";WITH cte_shipments AS (
                                        SELECT S.[Id] [ShipmentId]
                                        FROM [dbo].[FreightMovementItems] FMI 
                                            INNER JOIN [dbo].Shipments S ON S.FreightMovementId = FMI.FreightMovementId
                                        WHERE FMI.PurchaseOrderId = @PurchaseOrderId {queryBuilder.ToString()}
                                        GROUP BY S.[Id]
                                    )
                                    ,cte_raw as (
                                        SELECT SR.ShipmentId, 
                                            '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], PL.LocCode [PortofLoadingCode],
                                            '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], PD.LocCode [PortofDischargeCode], 
                                            FM.GoodsReady, FM.Tags, S.[Tags] [sTags],
                                            FM.TransactionType, FM.IncoTerms, FM.ShipmentType, FM.Name, FM.LoadType, FM.Reference, PR.Name [Partner], CL.Name [Client], NULL [Products], SU.[Name] [Supplier], NULL [ContainerList]
                                        FROM cte_shipments SR
                                            INNER JOIN [dbo].[Shipments] S ON S.Id = SR.ShipmentId
                                            INNER JOIN [dbo].[Partnerships] P ON P.Id = S.PartnershipId
                                            LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                            LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                            INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                                            LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                            LEFT JOIN [dbo].[Organisations] SU ON SU.Id = AL.OrganisationId
                                            LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                            LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = S.PortOfLoadingId
                                            LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = S.PortOfDischargeId
                                    )
                                    SELECT CS.ShipmentId [Id],
                                        CS.PlaceofLoading, CS.[PortofLoadingCode], CS.PortofLoading, CS.[PortofDischargeCode], CS.PortofDischarge, CS.PlaceofDischarge,
                                        CS.GoodsReady, CS.IncoTerms, CS.ShipmentType, FM.Name, CS.LoadType, FM.Reference, CS.TransactionType, 
                                        CS.[Partner], CS.[Client], CS.Products, CS.[Supplier], 
                                        S.Tags, S.CollectionDate, S.Status, S.Stage,
                                        S.[ArrivedPOD], S.[ArrivedPODDate], S.[Booked], S.[BookedDate], S.[Collected], S.[CustomsClearence], S.[CustomsClearenceDate], 
                                        S.[Delivered], S.[DeliveryDate], S.[EquipmentTrackAvailable], S.[EstimatedCollectionDate], S.[InCustoms], S.[InTransit], 
                                        S.[InTransitDate], S.[IssueAtCustomCleared], S.[IssueAtCustoms], S.[IssueAtCustomsDate], S.[ShipmentTrackAvailable],
                                        S.[SCAC], S.[BillofLadingNumber], S.[IMO], S.[VesselName], S.[Latitude], S.[Longitude],
                                        S.[DepartedPOL],S.[DepartedPOLDate],S.[EstimatedDeliveryDate],S.[TrackinformationAdded], S.[ETA] [EstimatedArrivalPOD], S.[ETD] [EstimatedDeparturePOL], CS.ContainerList, 
                                        S.[IsRescheduled], S.[LastRescheduleTime], S.[Rescheduled]
                                    FROM cte_raw CS
                                            INNER JOIN [dbo].[Shipments] S ON S.Id = CS.ShipmentId
                                            INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                                    ORDER BY S.CreationDateInternal desc";




                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    IEnumerable<ShipmentInfo> query_result = await conn.QueryAsync<ShipmentInfo>(sQuery, new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId
                    });
                    IPagedList<ShipmentInfo> shipments = await query_result.ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                    return shipments;
                }
            }
        }
    }

}
