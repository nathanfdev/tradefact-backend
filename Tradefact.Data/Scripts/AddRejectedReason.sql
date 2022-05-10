IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO


                DECLARE @migrationsCount INT = (SELECT COUNT(*) FROM [dbo].[__EFMigrationsHistory])
                IF @migrationsCount = 0
                BEGIN
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                    VALUES (N'20200717120124_Initial-PostCleanup', N'3.1.4');

                END
                

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200717120124_Initial-PostCleanup', N'3.1.4');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [AspNetUsers] ADD [ExternalProverUUID] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200720093056_AddedExternalUserId', N'3.1.4');

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [AspNetUsers] ADD [RegistrationDate] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200721074750_AddedUserRegisrationDate', N'3.1.4');

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

CREATE TABLE [AspNetUserInvitations] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [UserId] nvarchar(450) NULL,
    [InviteType] int NOT NULL,
    [CompanyName] nvarchar(max) NULL,
    [GivenName] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_AspNetUserInvitations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserInvitations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_AspNetUserInvitations_UserId] ON [AspNetUserInvitations] ([UserId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200901100955_UserInvitationLog', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [FK_PurchaseOrders_Shipments_ShipmentId];

GO

DROP TABLE [PurchaseOrderLines];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Amount');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Amount];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CreationDate');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [CreationDate];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CurrencyId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [CurrencyId];

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'DeliveryDate');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [DeliveryDate];

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Discount');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Discount];

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Freight');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Freight];

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'LastModifiedOn');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [LastModifiedOn];

GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'OrderDate');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [OrderDate];

GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'PurchaseOrderId');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [PurchaseOrderId];

GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'PurchaseOrderName');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [PurchaseOrderName];

GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'PurchaseTypeId');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [PurchaseTypeId];

GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Remarks');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Remarks];

GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'SubTotal');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [SubTotal];

GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Tax');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Tax];

GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Timestamp');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Timestamp];

GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Total');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [PurchaseOrders] DROP COLUMN [Total];

GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

EXEC sp_rename N'[PurchaseOrders].[IsActive]', N'Active', N'COLUMN';

GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[collab].[Rooms]') AND [c].[name] = N'Id');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [collab].[Rooms] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [collab].[Rooms] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[collab].[Messages]') AND [c].[name] = N'Id');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [collab].[Messages] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [collab].[Messages] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'Id');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [Shipments] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Quotations]') AND [c].[name] = N'Id');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Quotations] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Quotations] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[QuotationRequests]') AND [c].[name] = N'Id');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [QuotationRequests] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [QuotationRequests] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'SupplierId');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [SupplierId] uniqueidentifier NOT NULL;

GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ShipmentId');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ShipmentId] uniqueidentifier NULL;

GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'LastModifiedOnInternal');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [LastModifiedOnInternal] datetime2 NOT NULL;

GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'LastChangeUser');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [LastChangeUser] nvarchar(128) NULL;

GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CreationDateInternal');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [CreationDateInternal] datetime2 NOT NULL;

GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CreatedByUser');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [CreatedByUser] nvarchar(128) NULL;

GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CompanyId');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [CompanyId] uniqueidentifier NOT NULL;

GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Id');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Active');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Active] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(1 AS bit) FOR [Active];

GO

ALTER TABLE [PurchaseOrders] ADD [CorrespncExternalReference] nvarchar(32) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [CorrespncInternalReference] nvarchar(32) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [GoodsReadyDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO

ALTER TABLE [PurchaseOrders] ADD [IncoTerms] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [PurchaseOrders] ADD [IncotermsVersion] nvarchar(max) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Language] nvarchar(4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [LoadType] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [PurchaseOrders] ADD [PlaceOfDispatchId] uniqueidentifier NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PlaceOfLoadingId] uniqueidentifier NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PortOfDischargeId] nvarchar(8) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PortOfLoadingId] nvarchar(8) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PurchaseOrderDate] nvarchar(max) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PurchaseOrderNumber] nvarchar(64) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PurchasingGroup] nvarchar(8) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Reference] nvarchar(32) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ShipmentType] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [PurchaseOrders] ADD [Tags] nvarchar(512) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [TransactionType] int NOT NULL DEFAULT 0;

GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductVariants]') AND [c].[name] = N'Id');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [ProductVariants] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [ProductVariants] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'Id');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [Products] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Partnerships]') AND [c].[name] = N'Id');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Partnerships] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [Partnerships] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FreightMovements]') AND [c].[name] = N'Id');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [FreightMovements] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [FreightMovements] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Documents]') AND [c].[name] = N'Id');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Documents] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [Documents] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserInvitations]') AND [c].[name] = N'Id');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserInvitations] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [AspNetUserInvitations] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Addresses]') AND [c].[name] = N'Id');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Addresses] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [Addresses] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

GO

CREATE TABLE [PurchaseOrderItems] (
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [ProductId] uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NULL,
    [IsProductVariant] bit NOT NULL DEFAULT CAST(0 AS bit),
    [SKU] nvarchar(64) NULL,
    [PurchaseOrderItemText] nvarchar(64) NULL,
    [OrderQuantity] bigint NOT NULL,
    [OrderQuantityUnit] nvarchar(32) NULL,
    [OrderPriceUnit] nvarchar(32) NULL,
    [NetPriceAmount] decimal(18, 4) NOT NULL,
    [NetPriceQuantity] int NOT NULL,
    [TaxCode] nvarchar(8) NULL,
    [TaxCountry] nvarchar(8) NULL,
    [TaxJurisdiction] nvarchar(max) NULL,
    [TaxDeterminationDate] nvarchar(max) NULL,
    [IsDeliveryComplete] bit NOT NULL DEFAULT CAST(0 AS bit),
    [IsFinallyInvoiced] bit NOT NULL DEFAULT CAST(0 AS bit),
    [PurchaseOrderItemCategory] nvarchar(8) NULL,
    [AccountAssignmentCategory] nvarchar(8) NULL,
    [PurchaseContract] nvarchar(64) NULL,
    [ItemNetWeight] decimal(18, 4) NOT NULL,
    [ItemWeightUnit] nvarchar(8) NULL,
    [ItemVolume] decimal(18, 4) NOT NULL,
    [ItemVolumeUnit] nvarchar(8) NULL,
    [PurchaseOrderItemId] uniqueidentifier NOT NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY ([PurchaseOrderId], [Id]),
    CONSTRAINT [FK_PurchaseOrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItems_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [PurchaseOrderNotes] (
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [TextObjectType] nvarchar(max) NULL,
    [Language] nvarchar(4) NULL,
    [PlainLongText] nvarchar(max) NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_PurchaseOrderNotes] PRIMARY KEY ([PurchaseOrderId], [Id]),
    CONSTRAINT [FK_PurchaseOrderNotes_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [PurchaseOrderItemNotes] (
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [PurchaseOrderItemId] uniqueidentifier NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [TextObjectType] nvarchar(max) NULL,
    [Language] nvarchar(4) NULL,
    [PlainLongText] nvarchar(max) NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_PurchaseOrderItemNotes] PRIMARY KEY ([PurchaseOrderId], [PurchaseOrderItemId], [Id]),
    CONSTRAINT [FK_PurchaseOrderItemNotes_PurchaseOrderItems_PurchaseOrderId_Id] FOREIGN KEY ([PurchaseOrderId], [Id]) REFERENCES [PurchaseOrderItems] ([PurchaseOrderId], [Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_PurchaseOrders_PlaceOfDispatchId] ON [PurchaseOrders] ([PlaceOfDispatchId]);

GO

CREATE INDEX [IX_PurchaseOrders_PlaceOfLoadingId] ON [PurchaseOrders] ([PlaceOfLoadingId]);

GO

CREATE INDEX [IX_PurchaseOrders_PortOfDischargeId] ON [PurchaseOrders] ([PortOfDischargeId]);

GO

CREATE INDEX [IX_PurchaseOrders_PortOfLoadingId] ON [PurchaseOrders] ([PortOfLoadingId]);

GO

CREATE INDEX [IX_PurchaseOrderItemNotes_PurchaseOrderId_Id] ON [PurchaseOrderItemNotes] ([PurchaseOrderId], [Id]);

GO

CREATE INDEX [IX_PurchaseOrderItems_ProductId] ON [PurchaseOrderItems] ([ProductId]);

GO

CREATE INDEX [IX_PurchaseOrderItems_ProductVariantId] ON [PurchaseOrderItems] ([ProductVariantId]);

GO

ALTER TABLE [PurchaseOrders] ADD CONSTRAINT [FK_PurchaseOrders_Addresses_PlaceOfDispatchId] FOREIGN KEY ([PlaceOfDispatchId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrders] ADD CONSTRAINT [FK_PurchaseOrders_Addresses_PlaceOfLoadingId] FOREIGN KEY ([PlaceOfLoadingId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrders] ADD CONSTRAINT [FK_PurchaseOrders_Locations_PortOfDischargeId] FOREIGN KEY ([PortOfDischargeId]) REFERENCES [Locations] ([LocCode]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrders] ADD CONSTRAINT [FK_PurchaseOrders_Locations_PortOfLoadingId] FOREIGN KEY ([PortOfLoadingId]) REFERENCES [Locations] ([LocCode]) ON DELETE NO ACTION;

GO

ALTER TABLE [PurchaseOrders] ADD CONSTRAINT [FK_PurchaseOrders_Shipments_ShipmentId] FOREIGN KEY ([ShipmentId]) REFERENCES [Shipments] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200918095546_Initial-PurchaseOrder', N'3.1.4');

GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'SupplierId');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [SupplierId] uniqueidentifier NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Stage] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [PurchaseOrders] ADD [Status] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201001081457_AddPurchaseOrderStatus', N'3.1.4');

GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

CREATE TABLE [OrganisationRegistrations] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyLegalName] nvarchar(250) NULL,
    [Country] nvarchar(250) NULL,
    [Address] nvarchar(250) NULL,
    [City] nvarchar(250) NULL,
    [WebAddress] nvarchar(250) NULL,
    [BusinessId] nvarchar(250) NULL,
    [BusinessRegDocument] varbinary(max) NULL,
    [RegistrationStatus] int NOT NULL,
    [CompanyBio] nvarchar(max) NULL,
    [OrganisationId] uniqueidentifier NULL,
    CONSTRAINT [PK_OrganisationRegistrations] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [OrganisationRegistrationUsers] (
    [OrganisationRegistrationId] uniqueidentifier NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(250) NULL,
    [Email] nvarchar(250) NULL,
    [Role] int NOT NULL,
    [IsAdmin] bit NOT NULL,
    CONSTRAINT [PK_OrganisationRegistrationUsers] PRIMARY KEY ([OrganisationRegistrationId], [Id]),
    CONSTRAINT [FK_OrganisationRegistrationUsers_OrganisationRegistrations_OrganisationRegistrationId] FOREIGN KEY ([OrganisationRegistrationId]) REFERENCES [OrganisationRegistrations] ([Id]) ON DELETE CASCADE
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201015155656_CreateNewRegistrationTables', N'3.1.4');

GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201021135541_POItemIdRemove', N'3.1.4');

GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderItems]') AND [c].[name] = N'PurchaseOrderItemId');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderItems] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [PurchaseOrderItems] DROP COLUMN [PurchaseOrderItemId];

GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201021144329_POItemIdRemove2', N'3.1.4');

GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201021164431_POItemIdRemove3', N'3.1.4');

GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrganisationRegistrations]') AND [c].[name] = N'BusinessRegDocument');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [OrganisationRegistrations] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [OrganisationRegistrations] DROP COLUMN [BusinessRegDocument];

GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrganisationRegistrations]') AND [c].[name] = N'RegistrationStatus');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [OrganisationRegistrations] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [OrganisationRegistrations] ALTER COLUMN [RegistrationStatus] int NOT NULL;

GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrganisationRegistrations]') AND [c].[name] = N'OrganisationId');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [OrganisationRegistrations] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [OrganisationRegistrations] ALTER COLUMN [OrganisationId] uniqueidentifier NULL;

GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrganisationRegistrations]') AND [c].[name] = N'CompanyBio');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [OrganisationRegistrations] DROP CONSTRAINT [' + @var51 + '];');
ALTER TABLE [OrganisationRegistrations] ALTER COLUMN [CompanyBio] nvarchar(max) NULL;

GO

ALTER TABLE [OrganisationRegistrations] ADD [BusinessRegDocumentName] nvarchar(max) NULL;

GO

ALTER TABLE [OrganisationRegistrations] ADD [BusinessRegDocumentUrl] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201022090419_updatedocumenttype', N'3.1.4');

GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderItems]') AND [c].[name] = N'OrderQuantity');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderItems] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [OrderQuantity] int NOT NULL;

GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderItems]') AND [c].[name] = N'OrderPriceUnit');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderItems] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [OrderPriceUnit] decimal(18, 4) NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201022115728_POItemQtyPrice', N'3.1.4');

GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [Documents] ADD [IsRichText] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [Documents] ADD [RichTextData] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201023105526_AddRichTextOption', N'3.1.4');

GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Tags');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Tags] nvarchar(512) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Accepted] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [AcceptedDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Cancelled] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [CancelledDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Completed] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [CompletedDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [CurrencyId] nvarchar(16) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [CustomsBrokerageRequired] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [DateOfIssue] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO

ALTER TABLE [PurchaseOrders] ADD [ExchangeRate] decimal(18, 4) NOT NULL DEFAULT 0.0;

GO

ALTER TABLE [PurchaseOrders] ADD [InProduction] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [InProductionDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [InsuranceCurrency] nvarchar(8) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [InsuranceRequired] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [InsuranceValue] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [InverseExchangeRate] decimal(18, 4) NOT NULL DEFAULT 0.0;

GO

ALTER TABLE [PurchaseOrders] ADD [Notes] nvarchar(max) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [NumberOfItems] int NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PaymentTerms] nvarchar(max) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [PreShipment] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [PreShipmentDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Rejected] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [RejectedDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ShippedDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Shipping] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [Signature] nvarchar(max) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Submitted] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [SubmittedDate] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [TaxRate] decimal(18, 4) NOT NULL DEFAULT 0.0;

GO

ALTER TABLE [PurchaseOrders] ADD [TaxRateDescription] nvarchar(256) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ValidForDays] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [PurchaseOrders] ADD [BaseCurrency_CurrencyId] nvarchar(12) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [BaseCurrency_DiscountAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [BaseCurrency_NetAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [BaseCurrency_TaxAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [BaseCurrency_TotalAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Total_CurrencyId] nvarchar(12) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Total_DiscountAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Total_NetAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Total_TaxAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [Total_TotalAmount] decimal(18, 4) NULL;

GO

CREATE TABLE [PurchaseOrderAdditionalCharges] (
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [Description] nvarchar(256) NULL,
    [Id] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedByUser] nvarchar(max) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(max) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [Quantity] int NOT NULL,
    [BaseCurrency_CurrencyId] nvarchar(12) NULL,
    [BaseCurrency_NetAmount] decimal(18, 4) NULL,
    [UnitPrice] decimal(18, 4) NOT NULL,
    [BaseCurrency_TaxAmount] decimal(18, 4) NULL,
    [TaxRate] decimal(18, 4) NOT NULL,
    [BaseCurrency_TotalAmount] decimal(18, 4) NULL,
    [BaseCurrency_DiscountAmount] decimal(18, 4) NULL,
    [Margin] decimal(18, 4) NOT NULL,
    [Total_CurrencyId] nvarchar(12) NULL,
    [Total_NetAmount] decimal(18, 4) NULL,
    [Total_TaxAmount] decimal(18, 4) NULL,
    [Total_TotalAmount] decimal(18, 4) NULL,
    [Total_DiscountAmount] decimal(18, 4) NULL,
    CONSTRAINT [PK_PurchaseOrderAdditionalCharges] PRIMARY KEY ([PurchaseOrderId], [Id]),
    CONSTRAINT [FK_PurchaseOrderAdditionalCharges_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201029220802_POAdditionalFields', N'3.1.4');

GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'PurchaseOrderDate');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [PurchaseOrderDate] datetime2 NULL;

GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'GoodsReadyDate');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [GoodsReadyDate] datetime2 NULL;

GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'DateOfIssue');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var58 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [DateOfIssue] datetime2 NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [AdditionalInformation] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201102104919_POGoodsReadyNullable', N'3.1.4');

GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderAdditionalCharges]') AND [c].[name] = N'Margin');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderAdditionalCharges] DROP CONSTRAINT [' + @var59 + '];');
ALTER TABLE [PurchaseOrderAdditionalCharges] DROP COLUMN [Margin];

GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderAdditionalCharges]') AND [c].[name] = N'TaxRate');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderAdditionalCharges] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [PurchaseOrderAdditionalCharges] DROP COLUMN [TaxRate];

GO

EXEC sp_rename N'[PurchaseOrderAdditionalCharges].[BaseCurrency_TotalAmount]', N'BaseCurrencyTotal_TotalAmount', N'COLUMN';

GO

EXEC sp_rename N'[PurchaseOrderAdditionalCharges].[BaseCurrency_TaxAmount]', N'BaseCurrencyTotal_TaxAmount', N'COLUMN';

GO

EXEC sp_rename N'[PurchaseOrderAdditionalCharges].[BaseCurrency_NetAmount]', N'BaseCurrencyTotal_NetAmount', N'COLUMN';

GO

EXEC sp_rename N'[PurchaseOrderAdditionalCharges].[BaseCurrency_DiscountAmount]', N'BaseCurrencyTotal_DiscountAmount', N'COLUMN';

GO

EXEC sp_rename N'[PurchaseOrderAdditionalCharges].[BaseCurrency_CurrencyId]', N'BaseCurrencyTotal_CurrencyId', N'COLUMN';

GO

EXEC sp_rename N'[PurchaseOrderAdditionalCharges].[UnitPrice]', N'Rate', N'COLUMN';

GO

ALTER TABLE [PurchaseOrderAdditionalCharges] ADD [Type] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201102223120_POAdditionalCharges', N'3.1.4');

GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [ShipmentDocuments] ADD [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

ALTER TABLE [ProductDocuments] ADD [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201104125322_AddActiveFlagsToDocumentMapping', N'3.1.4');

GO

CREATE TABLE [PurchaseOrderDocuments] (
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [DocumentId] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_PurchaseOrderDocuments] PRIMARY KEY ([PurchaseOrderId], [DocumentId]),
    CONSTRAINT [FK_PurchaseOrderDocuments_Documents_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Documents] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseOrderDocuments_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_PurchaseOrderDocuments_DocumentId] ON [PurchaseOrderDocuments] ([DocumentId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201105122235_PODocuments', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] ADD [Containers] nvarchar(512) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201105133240_POAddedContainers', N'3.1.4');

GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Submitted');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var62 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Submitted] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [Submitted];

GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Shipping');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var63 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Shipping] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [Shipping];

GO

DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Rejected');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var64 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Rejected] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [Rejected];

GO

DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'PreShipment');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var65 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [PreShipment] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [PreShipment];

GO

DECLARE @var66 sysname;
SELECT @var66 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'InProduction');
IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var66 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [InProduction] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [InProduction];

GO

DECLARE @var67 sysname;
SELECT @var67 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Completed');
IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var67 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Completed] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [Completed];

GO

DECLARE @var68 sysname;
SELECT @var68 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Cancelled');
IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var68 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Cancelled] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [Cancelled];

GO

DECLARE @var69 sysname;
SELECT @var69 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'Accepted');
IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var69 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [Accepted] bit NOT NULL;
ALTER TABLE [PurchaseOrders] ADD DEFAULT CAST(0 AS bit) FOR [Accepted];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201106100225_POBitDefaultFlip', N'3.1.4');

GO

CREATE TABLE [PurchaseOrderAttachedProductDocuments] (
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    [DocumentId] uniqueidentifier NOT NULL,
    [PurchaseOrderProductId] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_PurchaseOrderAttachedProductDocuments] PRIMARY KEY ([PurchaseOrderId], [DocumentId]),
    CONSTRAINT [FK_PurchaseOrderAttachedProductDocuments_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseOrderAttachedProductDocuments_ProductDocuments_PurchaseOrderProductId_DocumentId] FOREIGN KEY ([PurchaseOrderProductId], [DocumentId]) REFERENCES [ProductDocuments] ([ProductId], [DocumentId]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_PurchaseOrderAttachedProductDocuments_PurchaseOrderProductId_DocumentId] ON [PurchaseOrderAttachedProductDocuments] ([PurchaseOrderProductId], [DocumentId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201109091900_PO-AttachedProductDocuments', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] ADD [HSCodes] nvarchar(512) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [LogisticsNotes] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201110113611_PO-ShippingData', N'3.1.4');

GO

DECLARE @var70 sysname;
SELECT @var70 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var70 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [Partnerships] ADD [PartnershipTypeId] uniqueidentifier NULL;

GO

CREATE TABLE [PartnershipType] (
    [Id] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedByUser] nvarchar(max) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(max) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [Name] nvarchar(max) NULL,
    [ProviderTypeId] int NOT NULL,
    [ClientTypeId] int NOT NULL,
    CONSTRAINT [PK_PartnershipType] PRIMARY KEY ([Id])
);

GO

CREATE INDEX [IX_Partnerships_PartnershipTypeId] ON [Partnerships] ([PartnershipTypeId]);

GO

ALTER TABLE [Partnerships] ADD CONSTRAINT [FK_Partnerships_PartnershipType_PartnershipTypeId] FOREIGN KEY ([PartnershipTypeId]) REFERENCES [PartnershipType] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201110160538_AddPartnershipTypes', N'3.1.4');

GO

ALTER TABLE [Partnerships] DROP CONSTRAINT [FK_Partnerships_PartnershipType_PartnershipTypeId];

GO

ALTER TABLE [PartnershipType] DROP CONSTRAINT [PK_PartnershipType];

GO

DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'Id');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var71 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [Id];

GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'CreatedByUser');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var72 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [CreatedByUser];

GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'CreationDate');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var73 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [CreationDate];

GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'CreationDateInternal');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var74 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [CreationDateInternal];

GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'IsActive');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [IsActive];

GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'LastChangeUser');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var76 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [LastChangeUser];

GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'LastModifiedOn');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [LastModifiedOn];

GO

DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'LastModifiedOnInternal');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var78 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [LastModifiedOnInternal];

GO

DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnershipType]') AND [c].[name] = N'Timestamp');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [PartnershipType] DROP CONSTRAINT [' + @var79 + '];');
ALTER TABLE [PartnershipType] DROP COLUMN [Timestamp];

GO

DECLARE @var80 sysname;
SELECT @var80 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var80 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var80 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

ALTER TABLE [PartnershipType] ADD [PartnershipTypeId] int NOT NULL DEFAULT 0;

GO

DROP INDEX [IX_Partnerships_PartnershipTypeId] ON [Partnerships];
DECLARE @var81 sysname;
SELECT @var81 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Partnerships]') AND [c].[name] = N'PartnershipTypeId');
IF @var81 IS NOT NULL EXEC(N'ALTER TABLE [Partnerships] DROP CONSTRAINT [' + @var81 + '];');
ALTER TABLE [Partnerships] ALTER COLUMN [PartnershipTypeId] int NOT NULL;
CREATE INDEX [IX_Partnerships_PartnershipTypeId] ON [Partnerships] ([PartnershipTypeId]);

GO

ALTER TABLE [PartnershipType] ADD CONSTRAINT [PK_PartnershipType] PRIMARY KEY ([PartnershipTypeId]);

GO

ALTER TABLE [Partnerships] ADD CONSTRAINT [FK_Partnerships_PartnershipType_PartnershipTypeId] FOREIGN KEY ([PartnershipTypeId]) REFERENCES [PartnershipType] ([PartnershipTypeId]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201112112707_AddPartnershiptType2', N'3.1.4');

GO

DECLARE @var82 sysname;
SELECT @var82 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
IF @var82 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var82 + '];');
ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

GO

DECLARE @var83 sysname;
SELECT @var83 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'InsuranceRequired');
IF @var83 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var83 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [InsuranceRequired] bit NULL;

GO

DECLARE @var84 sysname;
SELECT @var84 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CustomsBrokerageRequired');
IF @var84 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var84 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [CustomsBrokerageRequired] bit NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201112120859_AddnullableBool', N'3.1.4');

GO

CREATE TABLE [PurchaseOrderEvents] (
    [Id] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [EventId] int NOT NULL,
    [EventType] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [ActionedBy] uniqueidentifier NOT NULL,
    [ActionDate] datetime2 NOT NULL,
    [PurchaseOrderId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_PurchaseOrderEvents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseOrderEvents_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_PurchaseOrderEvents_PurchaseOrderId] ON [PurchaseOrderEvents] ([PurchaseOrderId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201112164820_PO-EventsData', N'3.1.4');

GO

ALTER TABLE [PurchaseOrderEvents] DROP CONSTRAINT [PK_PurchaseOrderEvents];

GO

DECLARE @var85 sysname;
SELECT @var85 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderEvents]') AND [c].[name] = N'EventId');
IF @var85 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderEvents] DROP CONSTRAINT [' + @var85 + '];');
ALTER TABLE [PurchaseOrderEvents] DROP COLUMN [EventId];

GO

ALTER TABLE [PurchaseOrderEvents] ADD [TEventId] int NOT NULL IDENTITY;

GO

ALTER TABLE [PurchaseOrderEvents] ADD CONSTRAINT [PK_PurchaseOrderEvents] PRIMARY KEY ([TEventId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201112170510_PO-EventsData2', N'3.1.4');

GO

ALTER TABLE [PurchaseOrderEvents] DROP CONSTRAINT [PK_PurchaseOrderEvents];

GO

DECLARE @var86 sysname;
SELECT @var86 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrderEvents]') AND [c].[name] = N'TEventId');
IF @var86 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrderEvents] DROP CONSTRAINT [' + @var86 + '];');
ALTER TABLE [PurchaseOrderEvents] DROP COLUMN [TEventId];

GO

ALTER TABLE [PurchaseOrderEvents] ADD [EventId] int NOT NULL IDENTITY;

GO

ALTER TABLE [PurchaseOrderEvents] ADD CONSTRAINT [PK_PurchaseOrderEvents] PRIMARY KEY ([EventId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201112170933_PO-EventsData3', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] ADD [TargetDeliveryDate] datetime2 NULL;

GO

DECLARE @var87 sysname;
SELECT @var87 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FreightMovements]') AND [c].[name] = N'InsuranceRequired');
IF @var87 IS NOT NULL EXEC(N'ALTER TABLE [FreightMovements] DROP CONSTRAINT [' + @var87 + '];');
ALTER TABLE [FreightMovements] ALTER COLUMN [InsuranceRequired] bit NULL;

GO

DECLARE @var88 sysname;
SELECT @var88 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FreightMovements]') AND [c].[name] = N'CustomsBrokerageRequired');
IF @var88 IS NOT NULL EXEC(N'ALTER TABLE [FreightMovements] DROP CONSTRAINT [' + @var88 + '];');
ALTER TABLE [FreightMovements] ALTER COLUMN [CustomsBrokerageRequired] bit NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201116125937_POTargetDeliveryDate', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] ADD [AdditionalSupplierInformation] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201120151454_POAdditionalSupplierInformation', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] ADD [ChargesTotal_CurrencyId] nvarchar(12) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ChargesTotal_DiscountAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ChargesTotal_NetAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ChargesTotal_TaxAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ChargesTotal_TotalAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ItemsTotal_CurrencyId] nvarchar(12) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ItemsTotal_DiscountAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ItemsTotal_NetAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ItemsTotal_TaxAmount] decimal(18, 4) NULL;

GO

ALTER TABLE [PurchaseOrders] ADD [ItemsTotal_TotalAmount] decimal(18, 4) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201124101333_POLineChargeTotals', N'3.1.4');

GO

DECLARE @var89 sysname;
SELECT @var89 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ItemsTotal_TotalAmount');
IF @var89 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var89 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ItemsTotal_TotalAmount] decimal(18, 4) NULL;

GO

DECLARE @var90 sysname;
SELECT @var90 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ItemsTotal_TaxAmount');
IF @var90 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var90 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ItemsTotal_TaxAmount] decimal(18, 4) NULL;

GO

DECLARE @var91 sysname;
SELECT @var91 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ItemsTotal_NetAmount');
IF @var91 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var91 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ItemsTotal_NetAmount] decimal(18, 4) NULL;

GO

DECLARE @var92 sysname;
SELECT @var92 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ItemsTotal_DiscountAmount');
IF @var92 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var92 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ItemsTotal_DiscountAmount] decimal(18, 4) NULL;

GO

DECLARE @var93 sysname;
SELECT @var93 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ItemsTotal_CurrencyId');
IF @var93 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var93 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ItemsTotal_CurrencyId] nvarchar(12) NULL;

GO

DECLARE @var94 sysname;
SELECT @var94 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ChargesTotal_TotalAmount');
IF @var94 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var94 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ChargesTotal_TotalAmount] decimal(18, 4) NULL;

GO

DECLARE @var95 sysname;
SELECT @var95 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ChargesTotal_TaxAmount');
IF @var95 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var95 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ChargesTotal_TaxAmount] decimal(18, 4) NULL;

GO

DECLARE @var96 sysname;
SELECT @var96 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ChargesTotal_NetAmount');
IF @var96 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var96 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ChargesTotal_NetAmount] decimal(18, 4) NULL;

GO

DECLARE @var97 sysname;
SELECT @var97 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ChargesTotal_DiscountAmount');
IF @var97 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var97 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ChargesTotal_DiscountAmount] decimal(18, 4) NULL;

GO

DECLARE @var98 sysname;
SELECT @var98 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'ChargesTotal_CurrencyId');
IF @var98 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var98 + '];');
ALTER TABLE [PurchaseOrders] ALTER COLUMN [ChargesTotal_CurrencyId] nvarchar(12) NULL;

GO

CREATE TABLE [QueuedTask] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Status] int NOT NULL,
    [Description] nvarchar(256) NULL,
    [Payload] nvarchar(max) NULL,
    [Result] nvarchar(max) NULL,
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    CONSTRAINT [PK_QueuedTask] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201201131900_QueuedTasks', N'3.1.4');

GO

ALTER TABLE [PurchaseOrders] ADD [RejectedReason] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201202100409_AddRejectedReason.sql', N'3.1.4');

GO

