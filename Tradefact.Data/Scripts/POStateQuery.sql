declare @OrganisationId UNIQUEIDENTIFIER = '92839fb2-cd9b-441c-9619-50e9d64bb899';
declare @search varchar(10) = null;
declare @PageSize int = 12;
declare @PageNumber int = 1;

DECLARE @POs TABLE
(   
    PurchaseOrderId UNIQUEIDENTIFIER NOT NULL,
    LastModifiedOnInternal DATETIME NULL
)

;WITH cte_purchaseorders AS (
    SELECT P.[Id] [PurchaseOrderId], P.LastModifiedOnInternal
    FROM [dbo].[PurchaseOrders] P
    WHERE P.[Active] = 1 AND P.CompanyId = @OrganisationId AND (P.Status = 10 OR P.Status = 20 OR P.Status = 30 OR P.Status = 40 OR P.Status = 50 OR P.Status = 100)  
)

INSERT INTO @POs (
    PurchaseOrderId, LastModifiedOnInternal
)
SELECT PurchaseOrderId, LastModifiedOnInternal
 FROM cte_purchaseorders PO
ORDER BY PO.LastModifiedOnInternal desc

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
    PO.PreShipment, PO.PreShipmentDate, PO.Cancelled, PO.CancelledDate,

    PO.LoadType, PO.IncoTerms, PO.ShipmentType, SU.Name [Provider], CL.Name [Client], PO.CreationDateInternal, PO.LastModifiedOnInternal, PO.CreatedByUser, NULL [Products],
    ISNULL(PO.TaxRate,0) [TaxRate], 
    ISNULL(PO.BaseCurrency_NetAmount,0) [BaseCurrency_NetAmount], ISNULL(PO.BaseCurrency_TaxAmount,0) [BaseCurrency_TaxAmount], ISNULL(PO.BaseCurrency_TotalAmount,0) [BaseCurrency_TotalAmount], 
    ISNULL(PO.Total_NetAmount,0) [Total_NetAmount], ISNULL(PO.Total_TaxAmount,0) [Total_TaxAmount], ISNULL(PO.Total_TotalAmount,0) [Total_TotalAmount], PO.NumberOfItems
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
OFFSET (@PageNumber-1)*@PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY

SELECT P.PurchaseOrderId [PurchaseOrderId], PS.Id [ScheduleId], PS.ConfirmedGoodsReadyDate, PS.[Description]
FROM @POs P 
        INNER JOIN [dbo].PurchaseOrderItemScheduleLines PS ON PS.PurchaseOrderId = P.PurchaseOrderId
GROUP BY P.PurchaseOrderId, PS.Id, PS.ConfirmedGoodsReadyDate, PS.[Description]

SELECT  FM.*, S.[Status], S.Booked, S.InTransit, s.Delivered, 
        S.PortOfLoadingId, '('+PL.CountryCode+') - '+ PL.Name [PortOfLoading],
        S.PortOfLoadingId, '('+PD.CountryCode+') - '+ PD.Name [PortOfDischarge]
 FROM (
    SELECT P.PurchaseOrderId [PurchaseOrderId], FMI.FreightMovementId
    FROM @POs P 
            INNER JOIN [dbo].FreightMovementItems FMI ON FMI.PurchaseOrderId = P.PurchaseOrderId
    GROUP BY P.PurchaseOrderId, FMI.FreightMovementId
) FM
    INNER JOIN [dbo].[Shipments] S ON S.FreightMovementId = FM.FreightMovementId
    LEFT JOIN [dbo].Locations PL ON PL.LocCode = CONVERT(varchar(36),S.PortOfLoadingId)
    LEFT JOIN [dbo].Locations PD ON PD.LocCode = CONVERT(varchar(36),S.PortOfDischargeId)

SELECT COUNT(PurchaseOrderId) FROM @POs




-- , cte_schedule_fms AS (
--     SELECT  PIS.PurchaseOrderId, PIS.ScheduleId, FMI.FreightMovementId
--         FROM cte_purchaseorders_schedules PIS
--             LEFT JOIN [dbo].[FreightMovementItems] FMI ON FMI.PurchaseOrderId = PIS.PurchaseOrderId AND FMI.PurchaseOrderItemScheduleLineId = PIS.ScheduleId
--     GROUP BY PIS.PurchaseOrderId, PIS.ScheduleId, FMI.FreightMovementId
-- )
-- SELECT * FROM cte_schedule_fms

-- SELECT * FROM [dbo].[FreightMovementItems] FMI WHERE FMI.PurchaseOrderId IS NOT NULL

-- SELECT FMs.PurchaseOrderId, FMs.ScheduleId, QR.FreightMovementId, QR.[State], COUNT(QR.[State]) [Count]
-- FROM cte_schedule_fms FMs
--      LEFT JOIN QuotationRequests QR ON QR.FreightMovementId = FMs.FreightMovementId
--  GROUP BY FMs.PurchaseOrderId, FMs.ScheduleId, QR.FreightMovementId, QR.[State]
 

-- -- ;WITH cte_purchaseorders_schedules AS (
-- --     SELECT P.PurchaseOrderId [PurchaseOrderId], PS.Id [ScheduleId]
-- --     FROM @POs P 
-- --         INNER JOIN [dbo].PurchaseOrderItemScheduleLines PS ON PS.PurchaseOrderId = P.PurchaseOrderId
-- --     GROUP BY P.PurchaseOrderId, PS.Id
-- -- )
-- -- , cte_schedule_lines AS (
-- --     SELECT  PIS.PurchaseOrderId, PIS.ScheduleId, CI.FreightMovementId
-- --         FROM cte_purchaseorders_schedules PIS
-- --             LEFT JOIN [dbo].[CargoItems] CI ON CI.PurchaseOrderItemScheduleLineId = PIS.ScheduleId
-- --     GROUP BY PIS.PurchaseOrderId, PIS.ScheduleId, CI.FreightMovementId
-- -- )
-- -- SELECT [PurchaseOrderId], COUNT([ScheduleId]) [Schedules] 
-- -- FROM cte_purchaseorders_schedules 
-- -- GROUP BY [PurchaseOrderId]


-- -- Quotation Request Status:   0 - Pending, 1 - Ready,  2 - Accepted
-- -- SELECT DT.PurchaseOrderId, QR.FreightMovementId, QR.[State], COUNT(QR.[State]) [Count]
-- --  FROM (
-- --     SELECT DT.PurchaseOrderId,	ScheduleId,	FreightMovementId
-- --     FROM (
-- --         SELECT PO.PurchaseOrderId [PurchaseOrderId], SL.Id [ScheduleId] 
-- --         FROM @POs PO 
-- --             INNER JOIN [dbo].PurchaseOrderItemScheduleLines SL ON SL.PurchaseOrderId = PO.PurchaseOrderId
-- --         GROUP BY PO.PurchaseOrderId, SL.Id
-- --     ) DT
-- --     LEFT JOIN [dbo].[CargoItems] CI ON CI.PurchaseOrderItemScheduleLineId = DT.ScheduleId
-- --     WHERE CI.FreightMovementId IS NOT NULL
-- --     GROUP BY DT.PurchaseOrderId, ScheduleId, FreightMovementId
-- -- ) DT
-- --     LEFT JOIN QuotationRequests QR ON QR.FreightMovementId = DT.FreightMovementId
-- -- GROUP BY DT.PurchaseOrderId, ScheduleId, QR.FreightMovementId, QR.[State]

-- -- SELECT DT.PurchaseOrderId, QR.FreightMovementId, QR.[State], COUNT(QR.[State]) [Count]
-- --  FROM (
-- --     SELECT DT.PurchaseOrderId,	ScheduleId,	FreightMovementId
-- --     FROM (
-- --         SELECT PO.PurchaseOrderId [PurchaseOrderId], SL.Id [ScheduleId] 
-- --         FROM @POs PO 
-- --             INNER JOIN [dbo].PurchaseOrderItemScheduleLines SL ON SL.PurchaseOrderId = PO.PurchaseOrderId
-- --         GROUP BY PO.PurchaseOrderId, SL.Id
-- --     ) DT
-- --     LEFT JOIN [dbo].[CargoItems] CI ON CI.PurchaseOrderItemScheduleLineId = DT.ScheduleId
-- --     GROUP BY DT.PurchaseOrderId, ScheduleId, FreightMovementId
-- -- ) DT
-- --     LEFT JOIN QuotationRequests QR ON QR.FreightMovementId = DT.FreightMovementId
-- -- GROUP BY DT.PurchaseOrderId, ScheduleId, QR.FreightMovementId, QR.[State]



-- -- SELECT * FROM  [dbo].CargoItems CI WHERE CI.PurchaseOrderItemScheduleLineId IS NOT NULL