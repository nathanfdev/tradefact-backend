ALTER TABLE [AspNetUserInvitations] ADD [ActivationStatus] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210415100144_InvitationAccountActivationStatus', N'3.1.4');
GO

