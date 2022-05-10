-- 20210316125801_RemoveUnusedOrderQuantityField

ALTER TABLE [dbo].[Products] DROP CONSTRAINT [DF__Products__OrderQ__7755B73D]
GO

ALTER TABLE [Products] DROP COLUMN [OrderQuantityMinimum];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210316125801_RemoveUnusedOrderQuantityField', N'3.1.4');
GO

-- 20210316141417_IncreaseIdentifierSize

ALTER TABLE [Products] ALTER COLUMN [Identifier_ePID] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_UPC] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_MPN] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_JAN] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_ISBN] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_GTIN] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_GPC] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_EAN] nvarchar(20) NULL;
GO

ALTER TABLE [Products] ALTER COLUMN [Identifier_ASIN] nvarchar(20) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210316141417_IncreaseIdentifierSize', N'3.1.4');
GO

-- 20210317004203_POScheduleLinesSet

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Countries_CountryofLoadingCode] FOREIGN KEY ([CountryofLoadingCode]) REFERENCES [Countries] ([Code2]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode] FOREIGN KEY ([PortOfLoadingCode]) REFERENCES [Locations] ([LocCode]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id] FOREIGN KEY ([PurchaseOrderId], [Id]) REFERENCES [PurchaseOrderItems] ([PurchaseOrderId], [Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210317004203_POScheduleLinesSet', N'3.1.4');

GO

-- 20210317071359_POScheduleLinesKeyFix

ALTER TABLE [PurchaseOrderItemScheduleLines] DROP CONSTRAINT [FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id];
GO

DROP INDEX [IX_PurchaseOrderItemScheduleLines_PurchaseOrderId_Id] ON [PurchaseOrderItemScheduleLines];
GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId] FOREIGN KEY ([PurchaseOrderId], [PurchaseOrderItemId]) REFERENCES [PurchaseOrderItems] ([PurchaseOrderId], [Id]) ON DELETE NO ACTION;
GO


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210317071359_POScheduleLinesKeyFix', N'3.1.4');

-- 20210317151248_AddSupplierReference
ALTER TABLE [PurchaseOrderItems] ADD [SupplierReference] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210317151248_AddSupplierReference', N'3.1.4');
GO

-- 20210326094244_AddCurrencyToOrg
ALTER TABLE [Organisations] ADD [Currency] nvarchar(max) NULL DEFAULT N'USD';

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210326094244_AddCurrencyToOrg', N'3.1.4');
GO

-- 20210330120329_InvitationOrgData

ALTER TABLE [AspNetUserInvitations] ADD [InviteRequestedByOrganisationId] uniqueidentifier NULL;

GO

ALTER TABLE [AspNetUserInvitations] ADD [InviteRequestedByUserId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210330120329_InvitationOrgData', N'3.1.4');

GO

-- 20210330161247_InvitationMemberOf

ALTER TABLE [AspNetUserInvitations] ADD [MemberOfOrganisationId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210330161247_InvitationMemberOf', N'3.1.4');

GO

-- 20210331232206_InvitationActivate


ALTER TABLE [AspNetUsers] ADD [InvitationId] uniqueidentifier NULL;

GO

ALTER TABLE [AspNetUserInvitations] ADD [LastActivationAttempt] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210331232206_InvitationActivate', N'3.1.4');

GO

-- 20210402151646_B2BConnections

CREATE TABLE [B2B_Connections] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [OrgansationId] uniqueidentifier NOT NULL,
    [LinkedOrganisationId] uniqueidentifier NOT NULL,
    [Tags] nvarchar(max) NULL,
    [Rating] decimal(18, 4) NOT NULL,
    [OrganisationId] uniqueidentifier NULL,
    CONSTRAINT [PK_B2B_Connections] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_B2B_Connections_Organisations_LinkedOrganisationId] FOREIGN KEY ([LinkedOrganisationId]) REFERENCES [Organisations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_B2B_Connections_Organisations_OrganisationId] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [B2B_ConnectionContacts] (
    [Id] uniqueidentifier NOT NULL,
    [ConnectionId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [FullName] nvarchar(250) NULL,
    [FirstName] nvarchar(250) NULL,
    [MiddleName] nvarchar(250) NULL,
    [LastName] nvarchar(250) NULL,
    [Title] nvarchar(250) NULL,
    [Salutation] nvarchar(250) NULL,
    [Department] nvarchar(250) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [LocationId] uniqueidentifier NOT NULL,
    [Notes] nvarchar(250) NULL,
    CONSTRAINT [PK_B2B_ConnectionContacts] PRIMARY KEY ([ConnectionId], [Id]),
    CONSTRAINT [FK_B2B_ConnectionContacts_B2B_Connections_ConnectionId] FOREIGN KEY ([ConnectionId]) REFERENCES [B2B_Connections] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [B2B_ConnectionContactEmails] (
    [Id] uniqueidentifier NOT NULL,
    [ContactId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [Email] nvarchar(250) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ConnectionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_B2B_ConnectionContactEmails] PRIMARY KEY ([ContactId], [Id]),
    CONSTRAINT [FK_B2B_ConnectionContactEmails_B2B_ConnectionContacts_ConnectionId_ContactId] FOREIGN KEY ([ConnectionId], [ContactId]) REFERENCES [B2B_ConnectionContacts] ([ConnectionId], [Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [B2B_ConnectionContactPhoneNumbers] (
    [Id] uniqueidentifier NOT NULL,
    [ContactId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [CountryCode] nvarchar(8) NULL,
    [Number] nvarchar(32) NULL,
    [AreaCode] nvarchar(8) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ConnectionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_B2B_ConnectionContactPhoneNumbers] PRIMARY KEY ([ContactId], [Id]),
    CONSTRAINT [FK_B2B_ConnectionContactPhoneNumbers_B2B_ConnectionContacts_ConnectionId_ContactId] FOREIGN KEY ([ConnectionId], [ContactId]) REFERENCES [B2B_ConnectionContacts] ([ConnectionId], [Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_B2B_ConnectionContactEmails_ConnectionId_ContactId] ON [B2B_ConnectionContactEmails] ([ConnectionId], [ContactId]);

GO

CREATE INDEX [IX_B2B_ConnectionContactPhoneNumbers_ConnectionId_ContactId] ON [B2B_ConnectionContactPhoneNumbers] ([ConnectionId], [ContactId]);

GO

CREATE INDEX [IX_B2B_Connections_LinkedOrganisationId] ON [B2B_Connections] ([LinkedOrganisationId]);

GO

CREATE INDEX [IX_B2B_Connections_OrganisationId] ON [B2B_Connections] ([OrganisationId]);

GO

CREATE INDEX [IX_B2B_Connections_OrganisationID] ON [B2B_Connections] ([OrgansationId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210402151646_B2BConnections', N'3.1.4');

GO

-- 20210413103119_POScheduleLinePlaceLoading

ALTER TABLE [PurchaseOrderItemScheduleLines] DROP CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode];
GO

DROP INDEX [IX_PurchaseOrderItemScheduleLines_PortOfLoadingCode] ON [PurchaseOrderItemScheduleLines];
GO

ALTER TABLE [PurchaseOrderItemScheduleLines] DROP COLUMN [PortOfLoadingCode];
GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD [PlaceOfLoadingId] uniqueidentifier NULL;
GO

CREATE INDEX [IX_PurchaseOrderItemScheduleLines_PlaceOfLoadingId] ON [PurchaseOrderItemScheduleLines] ([PlaceOfLoadingId]);
GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Addresses_PlaceOfLoadingId] FOREIGN KEY ([PlaceOfLoadingId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210413103119_POScheduleLinePlaceLoading', N'3.1.4');
GO

-- 20210413144329_B2BConnectionStatus
ALTER TABLE [B2B_Connections] ADD [ConnectionStatus] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210413144329_B2BConnectionStatus', N'3.1.4');

GO


-- 20210415100144_InvitationAccountActivationStatus
ALTER TABLE [AspNetUserInvitations] ADD [ActivationStatus] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210415100144_InvitationAccountActivationStatus', N'3.1.4');

GO

-- 20210419163549_POScheduleLineCargoItem
ALTER TABLE [CargoItems] ADD [PlaceOfLoadingId] uniqueidentifier NULL;

GO

ALTER TABLE [CargoItems] ADD [PurchaseOrderId] uniqueidentifier NULL;

GO

ALTER TABLE [CargoItems] ADD [PurchaseOrderItemId] uniqueidentifier NULL;

GO

ALTER TABLE [CargoItems] ADD [PurchaseOrderItemScheduleLineId] uniqueidentifier NULL;

GO

CREATE INDEX [IX_CargoItems_PlaceOfLoadingId] ON [CargoItems] ([PlaceOfLoadingId]);

GO

CREATE INDEX [IX_CargoItems_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId] ON [CargoItems] ([PurchaseOrderId], [PurchaseOrderItemId], [PurchaseOrderItemScheduleLineId]);

GO

ALTER TABLE [CargoItems] ADD CONSTRAINT [FK_CargoItems_Addresses_PlaceOfLoadingId] FOREIGN KEY ([PlaceOfLoadingId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [CargoItems] ADD CONSTRAINT [FK_CargoItems_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId] FOREIGN KEY ([PurchaseOrderId], [PurchaseOrderItemId]) REFERENCES [PurchaseOrderItems] ([PurchaseOrderId], [Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [CargoItems] ADD CONSTRAINT [FK_CargoItems_PurchaseOrderItemScheduleLines_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId] FOREIGN KEY ([PurchaseOrderId], [PurchaseOrderItemId], [PurchaseOrderItemScheduleLineId]) REFERENCES [PurchaseOrderItemScheduleLines] ([PurchaseOrderId], [PurchaseOrderItemId], [Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210419163549_POScheduleLineCargoItem', N'3.1.4');

GO

-- 20210422111528_ContainerTypesAirSeaRoad

ALTER TABLE [ContainerTypes] ADD [Air] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [ContainerTypes] ADD [Road] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [ContainerTypes] ADD [Sea] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210422111528_ContainerTypesAirSeaRoad', N'3.1.4');

GO

UPDATE [dbo].[ContainerTypes] SET Sea = 1

INSERT INTO [dbo].[ContainerTypes] (Code, [Description], ISOTypeGroup, ISOTypeGroupDescription, [Length], Height, Width, AdditionalInformation, Active,	Air, Road, Sea)
SELECT '00R1', 'Refrigerated', ISOTypeGroup, ISOTypeGroupDescription, [Length], Height, Width, 'Refrigerated Road Transport', Active,	0, 1, 0
 FROM [dbo].[ContainerTypes] WHERE Code = '00G0'
UNION
SELECT '00R0', 'Dry', ISOTypeGroup, ISOTypeGroupDescription, [Length], Height, Width, 'Dry Road Transport', Active,	0, 1, 0
 FROM [dbo].[ContainerTypes] WHERE Code = '00G0'

UPDATE [dbo].[ContainerTypes] SET Active = 1 WHERE Road = 1


-- 20210423103100_FMPlaceOfLoadingMultiple
ALTER TABLE [FreightMovements] ADD [PlaceOfLoadingMultiple] nvarchar(256) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210423103100_FMPlaceOfLoadingMultiple', N'3.1.4');

GO

-- 20210426134119_AddressGeoLocation
ALTER TABLE [Addresses] ADD [Position_Latitude] decimal(18, 4) NULL;

GO

ALTER TABLE [Addresses] ADD [Position_Longitude] decimal(18, 4) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210426134119_AddressGeoLocation', N'3.1.4');

GO

-- 20210427112406_ActivationChargebee
ALTER TABLE [Organisations] ADD [ChargebeeSubscriptionId] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210427112406_ActivationChargebee', N'3.1.4');

GO

-- 20210428115502_MoveScheduleIdToFMItem
ALTER TABLE [CargoItems] DROP CONSTRAINT [FK_CargoItems_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId];

GO

ALTER TABLE [CargoItems] DROP CONSTRAINT [FK_CargoItems_PurchaseOrderItemScheduleLines_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId];

GO

DROP INDEX [IX_CargoItems_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId] ON [CargoItems];

GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [FreightMovementItems] ADD [PurchaseOrderId] uniqueidentifier NULL;

GO

ALTER TABLE [FreightMovementItems] ADD [PurchaseOrderItemScheduleLineId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210428115502_MoveScheduleIdToFMItem', N'3.1.4');

GO

-- 20210429085546_PurchaseOrderDocumentType
ALTER TABLE [PurchaseOrderDocuments] ADD [DocumentType] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210429085546_PurchaseOrderDocumentType', N'3.1.4');

GO