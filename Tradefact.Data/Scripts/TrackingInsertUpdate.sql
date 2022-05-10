declare @bol varchar(20) = 'BOL1234'
declare @equipmentItemId varchar(20) = 'MSKU5902220'
declare @eventType varchar(32) = 'Gate Out Full'
declare @timeOfEvent datetime = getdate()
declare @voyage varchar(16) = 'AL1234'
declare @information varchar(256) = '3 pieces at 1196.00 kilograms booked on flight'
declare @location varchar(256) = 'Narita International Airport, JP [NRT]'

DECLARE @TrackingEvents TABLE(shipmentId UNIQUEIDENTIFIER, bol varchar(20), equipmentItemId varchar(30), eventType varchar(32))

INSERT INTO @TrackingEvents(shipmentId, bol, equipmentItemId, eventType)
SELECT S.Id, @bol, @equipmentItemId , @eventType
FROM [dbo].[Shipments] S WHERE S.BillofLadingNumber = @bol

BEGIN TRAN

    UPDATE SE
        SET SE.TimeOfEvent = @timeOfEvent, SE.Voyage = @voyage, SE.Information = @information
        FROM @TrackingEvents TE 
    INNER JOIN [dbo].[ShipmentEvents] SE ON SE.ShipmentId = TE.shipmentId AND SE.EquipmentItemId = TE.equipmentItemId AND SE.Activity = TE.eventType

    INSERT INTO [dbo].ShipmentEvents(ShipmentId, TrackingNumber, EquipmentItemId, EventId, TimeOfEvent, Activity, Information)
    SELECT TE.shipmentId, @bol, @equipmentItemId, NEWID(), @timeOfEvent, @eventType, @information
    FROM @TrackingEvents TE 
        LEFT JOIN [dbo].[ShipmentEvents] SE ON SE.ShipmentId = TE.shipmentId AND SE.EquipmentItemId = TE.equipmentItemId AND SE.Activity = TE.eventType
    WHERE SE.ShipmentId IS NULL

    SELECT * FROM [dbo].[ShipmentEvents] SE WHERE SE.TrackingNumber = @bol;

ROLLBACK TRAN