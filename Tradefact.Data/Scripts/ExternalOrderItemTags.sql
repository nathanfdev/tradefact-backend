ALTER TABLE [integration].[ExternalPurchaseOrderLineItems] ADD [Tags] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211116164120_ExternalOrderItemTags', N'3.1.4');

GO

