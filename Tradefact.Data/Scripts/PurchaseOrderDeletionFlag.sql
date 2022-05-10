ALTER TABLE [PurchaseOrders] ADD [Deleted] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [PurchaseOrders] ADD [DeletionDate] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210608083703_PurchaseOrderDeletionFlag', N'3.1.4');

GO

