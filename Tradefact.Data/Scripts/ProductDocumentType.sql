ALTER TABLE [ProductDocuments] ADD [DocumentType] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210525145453_ProductDocumentType', N'3.1.4');

GO

