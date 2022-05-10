
ALTER TABLE [AspNetUsers] ADD [InvitationId] uniqueidentifier NULL;

GO

ALTER TABLE [AspNetUserInvitations] ADD [LastActivationAttempt] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210331232206_InvitationActivate', N'3.1.4');

GO

