ALTER TABLE [Addresses] ADD [Position_Latitude] decimal(18, 4) NULL;

GO

ALTER TABLE [Addresses] ADD [Position_Longitude] decimal(18, 4) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210426134119_AddressGeoLocation', N'3.1.4');

GO

