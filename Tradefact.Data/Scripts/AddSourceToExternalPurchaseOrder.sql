ALTER TABLE [integration].[ExternalPurchaseOrders] ADD [Source] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211026125652_AddSourceToExternalPurchaseOrder', N'3.1.4');

GO

