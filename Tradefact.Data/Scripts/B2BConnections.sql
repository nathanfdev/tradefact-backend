CREATE TABLE [B2B_Connections] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [OrgansationId] uniqueidentifier NOT NULL,
    [LinkedOrganisationId] uniqueidentifier NOT NULL,
    [Tags] nvarchar(max) NULL,
    [Rating] decimal(18, 4) NOT NULL,
    [OrganisationId] uniqueidentifier NULL,
    CONSTRAINT [PK_B2B_Connections] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_B2B_Connections_Organisations_LinkedOrganisationId] FOREIGN KEY ([LinkedOrganisationId]) REFERENCES [Organisations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_B2B_Connections_Organisations_OrganisationId] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [B2B_ConnectionContacts] (
    [Id] uniqueidentifier NOT NULL,
    [ConnectionId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [FullName] nvarchar(250) NULL,
    [FirstName] nvarchar(250) NULL,
    [MiddleName] nvarchar(250) NULL,
    [LastName] nvarchar(250) NULL,
    [Title] nvarchar(250) NULL,
    [Salutation] nvarchar(250) NULL,
    [Department] nvarchar(250) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [LocationId] uniqueidentifier NOT NULL,
    [Notes] nvarchar(250) NULL,
    CONSTRAINT [PK_B2B_ConnectionContacts] PRIMARY KEY ([ConnectionId], [Id]),
    CONSTRAINT [FK_B2B_ConnectionContacts_B2B_Connections_ConnectionId] FOREIGN KEY ([ConnectionId]) REFERENCES [B2B_Connections] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [B2B_ConnectionContactEmails] (
    [Id] uniqueidentifier NOT NULL,
    [ContactId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [Email] nvarchar(250) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ConnectionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_B2B_ConnectionContactEmails] PRIMARY KEY ([ContactId], [Id]),
    CONSTRAINT [FK_B2B_ConnectionContactEmails_B2B_ConnectionContacts_ConnectionId_ContactId] FOREIGN KEY ([ConnectionId], [ContactId]) REFERENCES [B2B_ConnectionContacts] ([ConnectionId], [Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [B2B_ConnectionContactPhoneNumbers] (
    [Id] uniqueidentifier NOT NULL,
    [ContactId] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Timestamp] varbinary(max) NULL,
    [LastModifiedOn] datetimeoffset NOT NULL,
    [CreationDate] datetimeoffset NOT NULL,
    [CountryCode] nvarchar(8) NULL,
    [Number] nvarchar(32) NULL,
    [AreaCode] nvarchar(8) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ConnectionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_B2B_ConnectionContactPhoneNumbers] PRIMARY KEY ([ContactId], [Id]),
    CONSTRAINT [FK_B2B_ConnectionContactPhoneNumbers_B2B_ConnectionContacts_ConnectionId_ContactId] FOREIGN KEY ([ConnectionId], [ContactId]) REFERENCES [B2B_ConnectionContacts] ([ConnectionId], [Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_B2B_ConnectionContactEmails_ConnectionId_ContactId] ON [B2B_ConnectionContactEmails] ([ConnectionId], [ContactId]);

GO

CREATE INDEX [IX_B2B_ConnectionContactPhoneNumbers_ConnectionId_ContactId] ON [B2B_ConnectionContactPhoneNumbers] ([ConnectionId], [ContactId]);

GO

CREATE INDEX [IX_B2B_Connections_LinkedOrganisationId] ON [B2B_Connections] ([LinkedOrganisationId]);

GO

CREATE INDEX [IX_B2B_Connections_OrganisationId] ON [B2B_Connections] ([OrganisationId]);

GO

CREATE INDEX [IX_B2B_Connections_OrganisationID] ON [B2B_Connections] ([OrgansationId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210402151646_B2BConnections', N'3.1.4');

GO

