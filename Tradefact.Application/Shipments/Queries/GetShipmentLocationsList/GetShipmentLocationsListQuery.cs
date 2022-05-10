using Core.Common;
using Core.Enums;
using Core.Models.Criteria;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.FreightMovements.Queries.Model;
using Tradefact.Application.Models;
using Tradefact.Application.Shipments.Queries.GetShipmentList;
using X.PagedList;

namespace Tradefact.Application.Shipments.Queries.GetShipmentLocationsList
{
    public class GetShipmentLocationsListQuery : IRequest<IPagedList<ShipmentMapInfo>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public ShipmentSearchCriteria Criteria { get; set; }

        public class GetShipmentLocationsListQueryHandler : IRequestHandler<GetShipmentLocationsListQuery, IPagedList<ShipmentMapInfo>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetShipmentLocationsListQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<IPagedList<ShipmentMapInfo>> Handle(GetShipmentLocationsListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string organisation_type_switcher = request.OrganisationType == OrganisationTypeEnum.SHIPPER ? "P.ClientId" : "P.ProviderId";

                StringBuilder query_builder = new StringBuilder();

                query_builder.AppendLine(@$"
                    DECLARE @shipments TABLE (
                        ShipmentId UNIQUEIDENTIFIER NOT NULL,
                        FreightMovementId UNIQUEIDENTIFIER NOT NULL,
                        PartnershipId UNIQUEIDENTIFIER NOT NULL,
                        LastModifiedOnInternal DATETIME2 NOT NULL
                    )

                    ;WITH cte_shipments AS (
                            SELECT S.[Id] [ShipmentId], S.FreightMovementId, S.PartnershipId, S.LastModifiedOnInternal
                            FROM [dbo].[Shipments] S
                            LEFT JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                            INNER JOIN [dbo].[Partnerships] P ON P.Id = S.PartnershipId
                            WHERE {organisation_type_switcher} = @OrganisationId AND (S.Delivered = 0 OR S.LastModifiedOnInternal > DATEADD(day, -10, GETDATE()))
                    )");

                query_builder.AppendLine(@"
                        INSERT INTO @shipments (ShipmentId, FreightMovementId, PartnershipId, LastModifiedOnInternal)
                        SELECT ShipmentId, FreightMovementId, PartnershipId, LastModifiedOnInternal FROM cte_shipments");

                query_builder.AppendLine(@"
                                    SELECT  CS.ShipmentId [Id], CS.FreightMovementId,
                                        PL.Position_Latitude [PortofLoading_Latitude], PL.Position_Longitude [PortofLoading_Longitude],    
                                        PD.Position_Latitude [PortofDischarge_Latitude], PD.Position_Longitude [PortofDischarge_Longitude],
                                        AL.Position_Latitude [PlaceOfLoading_Latitude], AL.Position_Longitude [PlaceOfLoading_Longitude],    
                                        AD.Position_Latitude [PlaceOfDischarge_Latitude], AD.Position_Longitude [PlaceOfDischarge_Longitude],
                                            '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading],
                                            '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], 
                                            PL.LocCode [PortofLoadingCode],
                                            '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], 
                                            '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
                                            PD.LocCode [PortofDischargeCode],
                                            FM.ShipmentType, PR.Name [Partner], FM.Reference, S.[ArrivedPOD], S.[Collected], S.[CustomsClearence], 
                                            S.[Delivered], S.[EstimatedCollectionDate], S.[IssueAtCustoms],
                                            S.[DepartedPOL], S.[EstimatedDeliveryDate], S.[ETA] [EstimatedArrivalPOD], S.[ETD] [EstimatedDeparturePOL]
                                            FROM (  SELECT S.ShipmentId, FreightMovementId, PartnershipId, S.LastModifiedOnInternal
                                                    FROM  @shipments S
                                                        LEFT JOIN [dbo].[EquipmentAllocations] E ON E.ShipmentId = S.ShipmentId
                                                    GROUP BY S.ShipmentId, S.[FreightMovementId], S.PartnershipId, S.LastModifiedOnInternal
                                            ) CS
                                        INNER JOIN [dbo].[Shipments] S ON S.Id = CS.ShipmentId
                                        INNER JOIN [dbo].[Partnerships] P ON P.Id = CS.PartnershipId
                                        INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = CS.FreightMovementId
                                        LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                        LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                        LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                        LEFT JOIN [dbo].[Organisations] SU ON SU.Id = AL.OrganisationId
                                        LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                        LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = S.PortOfLoadingId
                                        LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = S.PortOfDischargeId

                                    SELECT  FMI.FreightMovementId, FMI.Id [FreightMovementItemId], 
                                            ISNULL(FMI.Schedule, FMI.ContainerType) [Description],
                                            CASE WHEN FMI.Schedule IS NOT NULL THEN 1 ELSE 0 END [IsPOSchedule] ,
                                            ISNULL(FMI.ConfirmedGoodsReadyDate, FMI.GoodsReady) [GoodsReady], FMI.PlaceOfLoadingId, 
                                            FMI.PurchaseOrderId, PO.PurchaseOrderNumber, PO.Reference [PurchaseOrderReference], PO.Tags [PurchaseOrderTagsRaw],
                                            AD.Id [PlaceOfLoading_Id], AD.Name [PlaceOfLoading_Name], AD.AddressLine1 [PlaceOfLoading_AddressLine1], AD.AddressLine2 [PlaceOfLoading_AddressLine2], AD.AddressLine3 [PlaceOfLoading_AddressLine3], AD.AddressLine4 [PlaceOfLoading_AddressLine4], AD.PostalCode [PlaceOfLoading_PostalCode],
                                            AD.Province [PlaceOfLoading_Province], AD.City [PlaceOfLoading_City], AD.County [PlaceOfLoading_County], AD.Position_Latitude  [PlaceOfLoading_Latitude], AD.Position_Longitude [PlaceOfLoading_Longitude], AD.CountryCode [PlaceOfLoading_CountryCode], C.Name [PlaceOfLoading_Country]
                                     FROM (
                                        SELECT FMI.FreightMovementId, FMI.Id, PSL.ConfirmedGoodsReadyDate, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], FM.GoodsReady, FMI.ContainerTypeCode, CT.[Description] [ContainerType], PSL.[Description] [Schedule],
                                        PSL.PurchaseOrderId, PSL.Id [ScheduleId]
                                        FROM [dbo].[FreightMovements] FM
                                            INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                            LEFT JOIN [dbo].ContainerTypes CT ON CT.Code = FMI.ContainerTypeCode
                                            LEFT JOIN  [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                        GROUP BY FMI.FreightMovementId, FMI.Id, PSL.[Description], PSL.ConfirmedGoodsReadyDate, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), FM.GoodsReady, FMI.ContainerTypeCode, CT.[Description], PSL.PurchaseOrderId, PSL.Id
                                    ) FMI
                                        INNER JOIN  [dbo].[FreightMovements] FM ON FM.Id = FMI.FreightMovementId
                                        LEFT JOIN  [dbo].[PurchaseOrders] PO ON PO.Id = FMI.PurchaseOrderId
                                        LEFT JOIN  [dbo].[Addresses] AD ON AD.Id = ISNULL(FMI.PlaceOfLoadingId, FM.PlaceOfLoadingId)
                                        LEFT JOIN [dbo].[Countries] C ON C.Code2 = AD.CountryCode");

                var sQuery = query_builder.ToString();

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        request.OrganisationId,
                    }, commandTimeout: 60))
                    {
                        List<ShipmentMapInfo> query_result_shipments = (await multi.ReadAsync<ShipmentMapInfo>()).ToList();

                        List<FreightMovementItemDTO> fmItems = (await multi.ReadAsync<FreightMovementItemDTO>()).ToList();

                        foreach (var shipment in query_result_shipments)
                        {
                            var selectedFmItems = fmItems.Where(i => i.FreightMovementId == shipment.FreightMovementId).ToList();

                            if (selectedFmItems != null && selectedFmItems.Count > 0)
                            {
                                shipment.FreightMovement = new FreightMovementResource
                                {
                                    FreightMovementId = shipment.FreightMovementId,
                                    Items = selectedFmItems.Adapt<List<FreightMovementItemResource>>(),
                                    NoLoadingLocations = selectedFmItems.Count
                                }; 
                            }
                        }

                        IPagedList<ShipmentMapInfo> shipment_with_map_info = await query_result_shipments.ToPagedListAsync(1, int.MaxValue);
                        return shipment_with_map_info;
                    }
                }
            }
        }
    }
}
