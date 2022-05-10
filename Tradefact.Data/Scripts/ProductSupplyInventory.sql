IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO


ALTER TABLE [Products] ADD [MinStockQuantity] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Products] ADD [NotifyStockQuantityBelow] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Products] ADD [OrderQuantityMaximum] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Products] ADD [OrderQuantityMinimum] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Products] ADD [StockQuantity] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Products] ADD [Identifier_ASIN] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_EAN] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_GPC] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_GTIN] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_ISBN] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_JAN] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_MPN] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_UPC] nvarchar(16) NULL;

GO

ALTER TABLE [Products] ADD [Identifier_ePID] nvarchar(16) NULL;

GO

CREATE TABLE [ProductSuppliers] (
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [SupplierId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [SupplierReference] nvarchar(max) NULL,
    [Price] decimal(18, 4) NULL,
    [OldPrice] decimal(18, 4) NULL,
    [CreatedByUser] nvarchar(max) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(max) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL
    CONSTRAINT [PK_ProductSuppliers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductSuppliers_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductSuppliers_Organisations_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Organisations] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_ProductSuppliers_ProductId] ON [ProductSuppliers] ([ProductId]);

GO

CREATE INDEX [IX_ProductSuppliers_SupplierId] ON [ProductSuppliers] ([SupplierId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210222174545_ProductSupplyInventory', N'3.1.4');

GO

