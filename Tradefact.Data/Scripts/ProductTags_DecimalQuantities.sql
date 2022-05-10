
DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderItems]') AND [c].[name] = N'OrderQuantity');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderItems] DROP CONSTRAINT [' + @var76 + '];');
ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [OrderQuantity] decimal(18, 4) NOT NULL;

GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'UnitsPerPackage');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [Products] ALTER COLUMN [UnitsPerPackage] decimal(18, 4) NOT NULL;

GO

ALTER TABLE [Products] ADD [Tags] nvarchar(512) NULL;

GO

DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FreightMovements]') AND [c].[name] = N'ConsignmentQuantity');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [FreightMovements] DROP CONSTRAINT [' + @var78 + '];');
ALTER TABLE [FreightMovements] ALTER COLUMN [ConsignmentQuantity] decimal(18, 4) NOT NULL;

GO

DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Qty');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var79 + '];');
ALTER TABLE [CargoItems] ALTER COLUMN [Qty] decimal(18, 4) NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211021135713_ProductTags_DecimalQuantities', N'3.1.4');

GO

