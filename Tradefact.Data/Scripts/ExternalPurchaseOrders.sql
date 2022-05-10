IF SCHEMA_ID(N'integration') IS NULL EXEC(N'CREATE SCHEMA [integration];');
GO

CREATE TABLE [integration].[ExternalPurchaseOrders] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [ExternalID] nvarchar(128) NULL,
    [PurchaseOrderNumber] nvarchar(128) NULL,
    [Reference] nvarchar(128) NULL,
    [OrderDate] datetime2 NOT NULL,
    [GoodsReadyDate] datetime2 NOT NULL,
    [DateOfIssue] datetime2 NOT NULL,
    [PlaceOfIssue] datetime2 NOT NULL,
    [Status] nvarchar(30) NULL,
    [CurrencyRate] decimal(18, 4) NOT NULL,
    [CurrencyCode] nvarchar(16) NULL,
    [SubTotal] decimal(18, 4) NOT NULL,
    [TotalTax] decimal(18, 4) NOT NULL,
    [Total] decimal(18, 4) NOT NULL,
    [PaymentTerms] nvarchar(512) NULL,
    [SupplierCode] nvarchar(36) NULL,
    [SupplierName] nvarchar(256) NULL,
    [Tags] nvarchar(512) NULL,
    [Received] datetime2 NOT NULL,
    [Imported] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ImportDate] datetime2 NOT NULL,
    [ImportUserId] uniqueidentifier NOT NULL,
    [ImportUserName] nvarchar(16) NULL,
    [GenericProductId] uniqueidentifier NOT NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_ExternalPurchaseOrders] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [integration].[ExternalPurchaseOrderLineItems] (
    [Id] uniqueidentifier NOT NULL,
    [ExternalPurchaseOrderId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [LineItemID] nvarchar(36) NULL,
    [SKU] nvarchar(64) NULL,
    [Description] nvarchar(128) NULL,
    [SupplierReference] nvarchar(128) NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18, 4) NOT NULL,
    [TaxType] nvarchar(32) NULL,
    [TaxAmount] decimal(18, 4) NOT NULL,
    [LineAmount] decimal(18, 4) NOT NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_ExternalPurchaseOrderLineItems] PRIMARY KEY ([ExternalPurchaseOrderId], [Id]),
    CONSTRAINT [FK_ExternalPurchaseOrderLineItems_ExternalPurchaseOrders_ExternalPurchaseOrderId] FOREIGN KEY ([ExternalPurchaseOrderId]) REFERENCES [integration].[ExternalPurchaseOrders] ([Id]) ON DELETE NO ACTION
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210709090838_ExternalPurchaseOrders', N'3.1.4');

GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [PlaceOfIssue] nvarchar(128) NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [OrderDate] datetime2 NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [ImportUserId] uniqueidentifier NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [ImportDate] datetime2 NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [GoodsReadyDate] datetime2 NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [GenericProductId] uniqueidentifier NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ALTER COLUMN [DateOfIssue] datetime2 NULL;
GO

ALTER TABLE [integration].[ExternalPurchaseOrders] ADD [OrganisationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210709114152_ExternalPurchaseOrdersNullsPlusOrg', N'3.1.4');
GO

