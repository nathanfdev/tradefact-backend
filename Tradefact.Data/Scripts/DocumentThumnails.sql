ALTER TABLE [Documents] ADD [ThumbnailGenerated] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Documents] ADD [ThumbnailUrl] nvarchar(512) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210629130434_DocumentThumnails', N'3.1.4');
GO

