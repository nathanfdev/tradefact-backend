ALTER TABLE [AspNetUsers] ADD [IsExposedToOtherOrganization] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210903053909_AddIsExposedToOtherOrgInUsers', N'3.1.4');

GO

