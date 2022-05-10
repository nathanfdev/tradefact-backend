DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[integration].[ExternalPurchaseOrders]') AND [c].[name] = N'ImportUserName');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [integration].[ExternalPurchaseOrders] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [ImportUserName] nvarchar(64) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211029121211_IncreaseImportUserNameCharLimit', N'3.1.4');

GO

