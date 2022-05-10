CREATE TABLE [Activities] (
    [Id] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedByUser] nvarchar(max) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(max) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [OrganisationId] uniqueidentifier NOT NULL,
    [Type] int NOT NULL,
    [Text1] nvarchar(max) NULL,
    [Text2] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Entity] int NOT NULL,
    [Data] nvarchar(max) NULL,
    CONSTRAINT [PK_Activities] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210927093209_AddActivity', N'3.1.4');

GO

