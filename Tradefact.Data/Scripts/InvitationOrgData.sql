ALTER TABLE [AspNetUserInvitations] ADD [InviteRequestedByOrganisationId] uniqueidentifier NULL;

GO

ALTER TABLE [AspNetUserInvitations] ADD [InviteRequestedByUserId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210330120329_InvitationOrgData', N'3.1.4');

GO

