ALTER TABLE [AspNetUserInvitations] ADD [MemberOfOrganisationId] uniqueidentifier NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210330161247_InvitationMemberOf', N'3.1.4');

GO

