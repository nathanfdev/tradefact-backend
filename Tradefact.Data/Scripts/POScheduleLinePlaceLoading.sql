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

