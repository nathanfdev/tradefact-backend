
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

