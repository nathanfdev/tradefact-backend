DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

CREATE TABLE [ProductSupplierCurrency] (
    [Id] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedByUser] nvarchar(max) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(max) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [ProductSupplierId] uniqueidentifier NOT NULL,
    [CurrencyCode] nvarchar(max) NULL,
    CONSTRAINT [PK_ProductSupplierCurrency] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductSupplierCurrency_ProductSuppliers_ProductSupplierId] FOREIGN KEY ([ProductSupplierId]) REFERENCES [ProductSuppliers] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_ProductSupplierCurrency_ProductSupplierId] ON [ProductSupplierCurrency] ([ProductSupplierId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210713084537_AddProductSupplierCurrencies', N'3.1.4');

GO

