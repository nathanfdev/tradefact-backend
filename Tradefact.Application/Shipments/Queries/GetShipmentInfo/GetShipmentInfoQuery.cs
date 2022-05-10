using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models.Shipment;
using Mapster;

namespace Tradefact.Application.Shipments.Queries.GetShipmentInfo
{
    public class GetShipmentInfoQuery : IRequest<ShipmentInfo>
    {
        public Guid ShipmentId { get; set; }
        public Guid? ProviderId { get; set; }
        public Guid? ClientId { get; set; }

        public class GetShipmentInfoQueryHandler : IRequestHandler<GetShipmentInfoQuery, ShipmentInfo>
        {
            private readonly IDbConnection _connection;

            public GetShipmentInfoQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }


            protected class shipmentPoNumbers
            {
                public Guid shipmentId;
                public string purchaseOrderNumber;
            }

            public async Task<ShipmentInfo> Handle(GetShipmentInfoQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder where_clause = new StringBuilder();
                if (request.ProviderId.HasValue) where_clause.Append($" AND P.ProviderId = @ProviderId");
                if (request.ClientId.HasValue) where_clause.Append($" AND P.ClientId = @ClientId");

                string sQuery = @$";WITH cte_shipments AS (
                                        SELECT S.[Id] [ShipmentId]
                                        FROM [dbo].[Shipments] S
                                        INNER JOIN [dbo].[Partnerships] P ON P.Id = S.PartnershipId
                                        WHERE S.id = @ShipmentId {where_clause.ToString()}  
                                    )
                                    ,cte_shipment_container_items AS (
                                        SELECT S.ShipmentId, STRING_AGG(E.ContainerNo, ',') AS Containers
                                        FROM cte_shipments S
                                            INNER JOIN [dbo].[EquipmentAllocations] E ON E.ShipmentId = S.ShipmentId
                                        GROUP BY S.ShipmentId
                                    )
                                    SELECT S.Id [Id], 
                                        '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], PL.LocCode [PortofLoadingCode],
                                        '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], PD.LocCode [PortofDischargeCode], 
                                        FM.GoodsReady,
                                        FM.TransactionType, FM.IncoTerms, FM.ShipmentType, S.FreightMovementId, S.QuotationRequestId, FM.Name, ISNULL(S.LoadType, FM.LoadType) [LoadType], FM.Reference, PR.Name [Partner], CL.Name [Client], SU.[Name] [Supplier], 
                                        S.Tags, S.CollectionDate, S.Status, S.Stage,
                                        S.[ArrivedPOD], S.[ArrivedPODDate], S.[Booked], S.[BookedDate], S.[Collected], S.[CustomsClearence], S.[CustomsClearenceDate], 
                                        S.[Delivered], S.[DeliveryDate], S.[EquipmentTrackAvailable], S.[EstimatedCollectionDate], S.[InCustoms], S.[InTransit], 
                                        S.[InTransitDate], S.[IssueAtCustomCleared], S.[IssueAtCustoms], S.[IssueAtCustomsDate], S.[ShipmentTrackAvailable],
                                        S.[SCAC], S.[BillofLadingNumber], S.[IMO], S.[VesselName], S.[Latitude], S.[Longitude],
                                        S.[DepartedPOL],S.[DepartedPOLDate],S.[EstimatedDeliveryDate],S.[TrackinformationAdded], S.[ETA] [EstimatedArrivalPOD], S.[ETD] [EstimatedDeparturePOL], S.[Route]
                                        ,EQ.Containers [ContainerList],
                                        S.[IsRescheduled], S.[LastRescheduleTime], S.[Rescheduled], S.[EquipmentTrackAvailable],
                                        PL.Position_Latitude [PortofLoading_Latitude], PL.Position_Longitude [PortofLoading_Longitude],    
                                        PD.Position_Latitude [PortofDischarge_Latitude], PD.Position_Longitude [PortofDischarge_Longitude],
                                        AL.Position_Latitude [PlaceOfLoading_Latitude], AL.Position_Longitude [PlaceOfLoading_Longitude],    
                                        AD.Position_Latitude [PlaceOfDischarge_Latitude], AD.Position_Longitude [PlaceOfDischarge_Longitude]

                                    FROM cte_shipments [CS]
                                        LEFT JOIN cte_shipment_container_items EQ ON EQ.ShipmentId = CS.ShipmentId
                                        INNER JOIN  [dbo].[Shipments] S ON S.Id = CS.ShipmentId
                                        INNER JOIN [dbo].[Partnerships] P ON P.Id = S.PartnershipId
                                        LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                        LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                        INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                                        LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                        LEFT JOIN [dbo].[Organisations] SU ON SU.Id = AL.OrganisationId
                                        LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                        LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = S.PortOfLoadingId
                                        LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = S.PortOfDischargeId

                                SELECT  QRS.*, 
                                        PO.PurchaseOrderNumber [PurchaseOrderNumber], PO.Reference [PurchaseOrderReference], 
                                        O.Id [SupplierId], O.Name [SupplierName],
                                        '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading],
                                        AL.Id [AddressID], AL.AddressLine1, AL.AddressLine2, AL.AddressLine3, AL.AddressLine4, AL.City, AL.PostalCode, C.Code2 [CountryCode], C.Name [Country]
                                FROM (
                                    SELECT CS.[ShipmentId], FM.Id [FreightMovementId], 
                                            PSL.[PurchaseOrderId] [PurchaseOrderId],
                                            ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady) [GoodsReady], PSL.[Description] [ScheduleName]
                                    FROM (  
                                                SELECT S.Id [ShipmentId], FreightMovementId, PartnershipId, S.LastModifiedOnInternal
                                                FROM [dbo].Shipments S
                                                WHERE S.Id = @ShipmentId
                                            ) CS
                                        INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = CS.FreightMovementId
                                        INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                        LEFT JOIN [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                    GROUP BY CS.ShipmentId, FM.Id, PSL.[PurchaseOrderId], ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady), PSL.[Description] 
                                ) QRS
                                    LEFT JOIN [dbo].[PurchaseOrders] PO ON PO.Id = QRS.PurchaseOrderId
                                    LEFT JOIN [dbo].[Organisations] O ON O.Id = PO.SupplierId
                                    LEFT JOIN [dbo].[Addresses] AL ON AL.Id = QRS.PlaceOfLoadingId
                                    LEFT JOIN [dbo].[Countries] C ON C.Code2 = AL.CountryCode
                                ORDER BY QRS.ShipmentId, PlaceOfLoadingId

                                SELECT DISTINCT S.Id [ShipmentId], PO.PurchaseOrderNumber
                                    FROM [dbo].Shipments S
                                        INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = s.FreightMovementId
                                        INNER JOIN [dbo].[PurchaseOrders] PO ON PO.Id = FMI.PurchaseOrderId
                                WHERE S.Id = @ShipmentId
";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();


                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @ShipmentId = request.ShipmentId,
                        @ProviderId = request.ProviderId.GetValueOrDefault(),
                        @ClientId = request.ClientId.GetValueOrDefault(),
                    }, commandTimeout: 60))
                    {
                        ShipmentInfo shipment = (await multi.ReadAsync<ShipmentInfo>()).FirstOrDefault();

                        if (shipment?.Id == null)
                        {
                            throw new NotFoundException(nameof(ShipmentInfo), request.ShipmentId);
                        }

                        ShipmentStateMachine sm = new ShipmentStateMachine(shipment.Status, shipment.Stage);

                        shipment.ShipmentStage = new ShipmentStages
                        {
                            Booked = true,
                            AwaitingCollectionDate = sm.CanAssignCollectionDate,
                            AwaitingCollection = (sm.CanMarkCollected || sm.CanAssignCollectionDate)
                        };

                        List<ShipmentScheduleInfoDTO> query_result_shipment_schedules_dto = (await multi.ReadAsync<ShipmentScheduleInfoDTO>()).ToList();
                        List<ShipmentScheduleInfo> query_result_shipment_schedules = query_result_shipment_schedules_dto.Adapt<List<ShipmentScheduleInfo>>();

                        foreach (var loc in query_result_shipment_schedules)
                        {
                            if (!shipment.LoadingLocations.Any(q=>q.LocationId == loc.Address.Id))
                            {
                                shipment.LoadingLocations.Add(
                                    new ShipmentLocationInfo
                                    {
                                        LocationId = loc.Address.Id,
                                        Address = loc.Address,
                                        SupplierName = loc.SupplierName
                                    }    
                                );
                            }
                        }

                        List<shipmentPoNumbers> query_po_numbers = (await multi.ReadAsync<shipmentPoNumbers>()).ToList();

                        shipment.PurchaseOrderNumbers = query_po_numbers
                            .Where(q => q.shipmentId == shipment.Id)
                            .Select(q => q.purchaseOrderNumber)
                            .ToList();

                        shipment.Schedules = query_result_shipment_schedules.OrderBy(o => o.GoodsReady).Where(q => q.ShipmentId == shipment.Id).ToList();
                        if (shipment.Schedules?.Count > 0)
                        {
                            shipment.GoodsReadyMin = shipment.Schedules.Min(s => s.GoodsReady);
                            shipment.GoodsReadyMax = shipment.Schedules.Max(s => s.GoodsReady);

                            shipment.GoodsReadyDates = shipment.Schedules.OrderBy(o => o.GoodsReady).Select(s => s.GoodsReady).Distinct().ToArray();
                            shipment.NoOfGoodsReadyDates = shipment.GoodsReadyDates.Count();
                        }
                        else
                        {
                            shipment.GoodsReadyMin = shipment.GoodsReady;
                            shipment.GoodsReadyMax = shipment.GoodsReady;
                            shipment.GoodsReadyDates = new DateTime[] { shipment.GoodsReady };
                            shipment.NoOfGoodsReadyDates = shipment.GoodsReadyDates.Count();
                        }
                        return shipment;
                    }
              
                }
            }
        }
    }
}
