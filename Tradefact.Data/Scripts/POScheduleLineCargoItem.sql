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

