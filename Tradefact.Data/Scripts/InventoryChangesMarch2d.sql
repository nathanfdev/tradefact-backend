ALTER TABLE [Products] ALTER COLUMN [SKU] nvarchar(150) NULL;
GO

ALTER TABLE [Products] ADD [Barcode] nvarchar(250) NULL;
GO

ALTER TABLE [Products] ADD [IncomingStockQuantity] int NOT NULL DEFAULT 0;
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
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [SupplierId] uniqueidentifier NOT NULL,
    [SupplierReference] nvarchar(max) NULL,
    [Price] decimal(18, 4) NULL,
    [OldPrice] decimal(18, 4) NULL,
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
VALUES (N'20210303131438_InventoryChangesMarch2d', N'3.1.4');
GO

ALTER TABLE [ProductSuppliers] ADD [Currency] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210304104109_AddCurrencyToProductSupplier', N'3.1.4');

GO

ALTER TABLE [ProductSuppliers] ADD [OrderQuantityMinimum] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210308153428_AddOrderQuantityMinimumToProductSupplier', N'3.1.4');

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[vwSuppliers] AS
WITH cte_suppliers AS
(
    SELECT 1 [Source], O.ParentId, O.Id [SupplierId], '' [Status], COUNT(A.Id) [Locations] 
    FROM [dbo].[Organisations] O 
        LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.id
    WHERE O.OrganisationTypeId = 2 AND O.Active = 1 --and A.Active = 1
    GROUP BY O.ParentId, O.Id    
    UNION
    SELECT 2 [Source], P.ClientId [ParentId], O.Id [SupplierId], AU.Status, COUNT(A.Id) [Locations] 
    FROM [dbo].[Organisations] O
        LEFT JOIN [Partnerships] P on P.ProviderId = O.Id
        LEFT JOIN [AspNetUsers] AU on AU.Email = O.ContactEmail
        LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.Id
    WHERE P.PartnershipTypeId = 2
    GROUP BY P.ClientId, O.Id, AU.Status
), 
cte_orders AS 
(
    SELECT CS.Source, CS.ParentId, CS.SupplierId, CS.Locations, CS.Status, COUNT(PO.Id) [ActiveOrders]
    FROM cte_suppliers CS
        LEFT JOIN PurchaseOrders PO ON PO.SupplierId = CS.SupplierId
    WHERE PO.Active = 1
    GROUP BY CS.Source, CS.ParentId, CS.SupplierId, CS.Locations, CS.Status
),
cte_supplier_orders AS
(
    SELECT CO.Source, CO.ParentId, CO.SupplierId, CO.[ActiveOrders], CO.Status, [Locations], SUM(CASE WHEN S.ID IS NULL THEN 0 ELSE 1 END) [ActiveShipments]
    FROM cte_orders CO
        LEFT JOIN [dbo].FreightMovements FM ON FM.SupplierId = CO.SupplierId
        LEFT JOIN [dbo].Shipments S ON S.FreightMovementId = FM.Id AND (S.Active = 1 AND ISNULL(S.Delivered,0) = 0)
    GROUP BY CO.Source, CO.ParentId, CO.SupplierId, CO.ActiveOrders, CO.Status, [Locations]
)
SELECT S.Source, S.ActiveOrders, O.Name, O.ContactName, O.ContactEmail, O.ContactTelephone, O.PaymentTerms, O.TCs, S.Status, S.Locations, S.ActiveShipments, S.ParentId, S.SupplierId [Id] 
FROM cte_supplier_orders S
    INNER JOIN [dbo].[Organisations] O  ON O.Id = S.SupplierId
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[vwBuyers] AS
WITH cte_buyers AS
(
    SELECT 1 [Source], O.ParentId, O.Id [BuyerId], '' [Status], COUNT(A.Id) [Locations] 
    FROM [dbo].[Organisations] O 
        LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.id
    WHERE O.OrganisationTypeId = 3 AND O.Active = 1 --and A.Active = 1
    GROUP BY O.ParentId, O.Id    
    UNION
    SELECT 2 [Source], P.ProviderId [ParentId], O.Id [BuyerId], AU.Status [Status], COUNT(A.Id) [Locations] 
    FROM [dbo].[Organisations] O
        LEFT JOIN [Partnerships] P on P.ClientId = O.Id
        LEFT JOIN [AspNetUsers] AU on AU.Email = O.ContactEmail
        LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.Id
    WHERE P.PartnershipTypeId = 2
    GROUP BY P.ProviderId, O.Id, AU.Status
), 
cte_buyer_orders AS
(
    SELECT CB.Source, CB.ParentId, CB.BuyerId, CB.Status, [Locations], SUM(CASE WHEN S.ID IS NULL THEN 0 ELSE 1 END) [ActiveShipments]
    FROM cte_buyers CB
        LEFT JOIN [dbo].FreightMovements FM ON FM.BuyerId = CB.BuyerId
        LEFT JOIN [dbo].Shipments S ON S.FreightMovementId = FM.Id AND (S.Active = 1 AND S.Delivered = 0)
    GROUP BY CB.Source, CB.ParentId, CB.Status, CB.BuyerId, [Locations]
)
SELECT 0 [ActiveOrders], S.Source, O.Name, O.ContactName, O.ContactEmail, O.ContactTelephone, O.PaymentTerms, O.TCs, S.Locations, S.Status, S.ActiveShipments, S.ParentId, S.BuyerId [Id] 
FROM cte_buyer_orders S
    INNER JOIN [dbo].[Organisations] O  ON O.Id = S.BuyerId
GO

