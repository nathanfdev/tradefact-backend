DECLARE @PurchaseOrderId UNIQUEIDENTIFIER = 'd174f762-b81c-47b1-8009-87073c83812f'

;WITH cte_shipments AS (
    SELECT S.[Id] [ShipmentId]
    FROM [dbo].[FreightMovementItems] FMI 
        INNER JOIN [dbo].Shipments S ON S.FreightMovementId = FMI.FreightMovementId
    WHERE FMI.PurchaseOrderId = @PurchaseOrderId
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
    CS.GoodsReady, CS.IncoTerms, CS.ShipmentType, CS.Name, CS.LoadType, CS.Reference, CS.TransactionType, 
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
ORDER BY S.CreationDateInternal desc