ALTER TABLE [FreightMovements] ADD [PlaceOfLoadingMultiple] nvarchar(256) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210423103100_FMPlaceOfLoadingMultiple', N'3.1.4');

GO

