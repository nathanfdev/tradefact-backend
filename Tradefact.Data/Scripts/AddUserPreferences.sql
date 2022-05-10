ALTER TABLE [AspNetUsers] ADD [UserPreferences] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210813143605_AddUserPreferences', N'3.1.4');

GO

