DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[integration].[ExternalPurchaseOrderLineItems]') AND [c].[name] = N'Quantity');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [integration].[ExternalPurchaseOrderLineItems] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [integration].[ExternalPurchaseOrderLineItems] ALTER COLUMN [Quantity] decimal(18, 4) NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211118140439_UpdateExternalOrderQuantityToDecimal', N'3.1.4');

GO

