ALTER TABLE [ProductDocuments] ADD [IsDefaultImage] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210624144302_AddMediaDocumentDefaultImage', N'3.1.4');

GO

