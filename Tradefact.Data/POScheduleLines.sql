CREATE TABLE [PurchaseOrderItemScheduleLines] (
    [Id] uniqueidentifier NOT NULL,
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [PurchaseOrderItemId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Description] nvarchar(250) NULL,
    [RequestedGoodsReadyDate] datetime2 NULL,
    [ConfirmedGoodsReadyDate] datetime2 NULL,
    [RequestedDeliveryDate] datetime2 NULL,
    [ConfirmedDeliveryDate] datetime2 NULL,
    [ScheduleLineOrderQuantity] int NOT NULL,
    [ScheduleLineCommittedQuantity] int NOT NULL,
    [OrderQuantityUnit] nvarchar(max) NULL,
    [ScheduleLineOrderWeight] decimal(18, 4) NOT NULL,
    [PortOfLoadingCode] nvarchar(8) NULL,
    [CountryofLoadingCode] nvarchar(4) NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_PurchaseOrderItemScheduleLines] PRIMARY KEY ([PurchaseOrderId], [PurchaseOrderItemId], [Id]),
    CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Countries_CountryofLoadingCode] FOREIGN KEY ([CountryofLoadingCode]) REFERENCES [Countries] ([Code2]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode] FOREIGN KEY ([PortOfLoadingCode]) REFERENCES [Locations] ([LocCode]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id] FOREIGN KEY ([PurchaseOrderId], [Id]) REFERENCES [PurchaseOrderItems] ([PurchaseOrderId], [Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_PurchaseOrderItemScheduleLines_CountryofLoadingCode] ON [PurchaseOrderItemScheduleLines] ([CountryofLoadingCode]);

GO

CREATE INDEX [IX_PurchaseOrderItemScheduleLines_PortOfLoadingCode] ON [PurchaseOrderItemScheduleLines] ([PortOfLoadingCode]);

GO

CREATE INDEX [IX_PurchaseOrderItemScheduleLines_PurchaseOrderId_Id] ON [PurchaseOrderItemScheduleLines] ([PurchaseOrderId], [Id]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210316124542_POScheduleLines', N'3.1.4');

GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Countries_CountryofLoadingCode] FOREIGN KEY ([CountryofLoadingCode]) REFERENCES [Countries] ([Code2]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode] FOREIGN KEY ([PortOfLoadingCode]) REFERENCES [Locations] ([LocCode]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210317004203_POScheduleLinesSet', N'3.1.4');

GO

ALTER TABLE [PurchaseOrderItemScheduleLines] ADD CONSTRAINT [FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId] FOREIGN KEY ([PurchaseOrderId], [PurchaseOrderItemId]) REFERENCES [PurchaseOrderItems] ([PurchaseOrderId], [Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210317071359_POScheduleLinesKeyFix', N'3.1.4');

GO

