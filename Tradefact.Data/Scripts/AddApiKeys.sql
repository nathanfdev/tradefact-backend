CREATE TABLE [integration].[ApiKeys] (
    [Key] nvarchar(36) NOT NULL,
    [OrganisationId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ApiKeys] PRIMARY KEY ([Key])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210928094058_AddApiKeys', N'3.1.4');

GO

