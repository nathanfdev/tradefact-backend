ALTER TABLE [Organisations] ADD [GenericSKUEnabled] bit NULL DEFAULT CAST(0 AS bit);

GO

CREATE TABLE [integration].[ExternalProducts] (
    [BatchId] uniqueidentifier NOT NULL,
    [SeqNo] int NOT NULL,
    [UserId] nvarchar(max) NULL,
    [ProductId] uniqueidentifier NULL,
    [CompanyId] uniqueidentifier NULL,
    [SKU] nvarchar(150) NULL,
    [Identifier_GTIN] nvarchar(20) NULL,
    [Identifier_UPC] nvarchar(20) NULL,
    [Identifier_EAN] nvarchar(20) NULL,
    [Identifier_JAN] nvarchar(20) NULL,
    [Identifier_ASIN] nvarchar(20) NULL,
    [Identifier_ISBN] nvarchar(20) NULL,
    [Identifier_MPN] nvarchar(20) NULL,
    [Identifier_ePID] nvarchar(20) NULL,
    [Identifier_GPC] nvarchar(20) NULL,
    [Description] nvarchar(512) NULL,
    [Name] nvarchar(250) NULL,
    [Nickname] nvarchar(250) NULL,
    [Dimensions_Height] decimal(18, 4) NULL,
    [Dimensions_Length] decimal(18, 4) NULL,
    [Dimensions_Scale] nvarchar(16) NULL,
    [Dimensions_Width] decimal(18, 4) NULL,
    [Dimensions_Weight] decimal(18, 4) NULL,
    [Dimensions_weightMeasurement] nvarchar(16) NULL,
    [GoodsType] nvarchar(250) NULL,
    [HsCode] nvarchar(250) NULL,
    [Packing] nvarchar(50) NULL,
    [UnitsPerPackage] decimal(18, 4) NOT NULL,
    [HazardousContents] int NOT NULL,
    [HazardClass] nvarchar(32) NULL,
    [HazardDescription] nvarchar(50) NULL,
    [HazardNotes] nvarchar(512) NULL,
    [Reference] nvarchar(256) NULL,
    [LithiumBatteryPacking] nvarchar(8) NULL,
    [MagneticFieldContained] bit NOT NULL,
    [Rotatable] bit NOT NULL,
    [Stackable] bit NOT NULL,
    [Barcode] nvarchar(250) NULL,
    [Tags] nvarchar(512) NULL,
    [StockQuantity] int NOT NULL,
    [MinStockQuantity] int NOT NULL,
    [NotifyStockQuantityBelow] int NOT NULL,
    [OrderQuantityMaximum] int NOT NULL,
    [IncomingStockQuantity] int NOT NULL,
    CONSTRAINT [PK_ExternalProducts] PRIMARY KEY ([BatchId], [SeqNo])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211026143802_OrganisationGenericSKUEnabled', N'3.1.4');

GO

