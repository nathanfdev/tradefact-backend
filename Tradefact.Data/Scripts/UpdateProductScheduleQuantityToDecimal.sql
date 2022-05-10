DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderItemScheduleLines]') AND [c].[name] = N'ScheduleLineCommittedQuantity');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderItemScheduleLines] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [PurchaseOrderItemScheduleLines] ALTER COLUMN [ScheduleLineCommittedQuantity] decimal(18, 4) NOT NULL;

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductSuppliers]') AND [c].[name] = N'OrderQuantityMinimum');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ProductSuppliers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ProductSuppliers] ALTER COLUMN [OrderQuantityMinimum] decimal(18, 4) NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211109103131_UpdateProductScheduleQuantityToDecimal', N'3.1.4');

GO

