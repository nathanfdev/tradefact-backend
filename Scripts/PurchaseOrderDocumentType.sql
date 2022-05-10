ALTER TABLE [PurchaseOrderDocuments] ADD [DocumentType] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210429085546_PurchaseOrderDocumentType', N'3.1.4');

GO

