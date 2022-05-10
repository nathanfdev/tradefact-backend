DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[integration].[ExternalPurchaseOrders]') AND [c].[name] = N'SupplierCode');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [integration].[ExternalPurchaseOrders] DROP CONSTRAINT [' + @var65 + '];');
ALTER TABLE [integration].[ExternalPurchaseOrders] DROP COLUMN [SupplierCode];

GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ADD [SupplierId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210922143806_ExternalPurchaseOrdersSupplierId', N'3.1.4');

GO

