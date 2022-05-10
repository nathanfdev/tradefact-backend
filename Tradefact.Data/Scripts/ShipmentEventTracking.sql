IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

CREATE TABLE [ShipmentEvents] (
    [ShipmentId] uniqueidentifier NOT NULL,
    [TrackingNumber] nvarchar(32) NULL,
    [EquipmentItemId] nvarchar(32) NOT NULL,
    [EventId] uniqueidentifier NOT NULL,
    [TimeOfEvent] datetime2 NOT NULL,
    [Voyage] nvarchar(16) NULL,
    [Activity] nvarchar(16) NULL,
    [Information] nvarchar(16) NULL,
    CONSTRAINT [PK_ShipmentEvents] PRIMARY KEY ([ShipmentId], [EquipmentItemId], [EventId]),
    CONSTRAINT [FK_ShipmentEvents_Shipments_ShipmentId] FOREIGN KEY ([ShipmentId]) REFERENCES [Shipments] ([Id]) ON DELETE CASCADE
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210209121613_ShipmentEventTracking', N'3.1.4');
GO

