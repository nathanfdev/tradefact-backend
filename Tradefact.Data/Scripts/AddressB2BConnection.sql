ALTER TABLE [Addresses] ADD [B2BConnectionId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210524113511_AddressB2BConnection', N'3.1.4');

GO

