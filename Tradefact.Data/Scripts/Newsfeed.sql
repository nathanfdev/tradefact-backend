CREATE TABLE [NewsFeeds] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [WeekNo] int NOT NULL,
    [Type] nvarchar(32) NULL,
    CONSTRAINT [PK_NewsFeeds] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [NewsFeedSections] (
    [NewsFeedId] uniqueidentifier NOT NULL,
    [Reference] nvarchar(36) NOT NULL,
    [Title] nvarchar(128) NULL,
    CONSTRAINT [PK_NewsFeedSections] PRIMARY KEY ([NewsFeedId], [Reference]),
    CONSTRAINT [FK_NewsFeedSections_NewsFeeds_NewsFeedId] FOREIGN KEY ([NewsFeedId]) REFERENCES [NewsFeeds] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [NewsFeedItems] (
    [NewsFeedId] uniqueidentifier NOT NULL,
    [NewsSectionReference] nvarchar(36) NOT NULL,
    [SeqNo] int NOT NULL,
    [Text] nvarchar(max) NULL,
    CONSTRAINT [PK_NewsFeedItems] PRIMARY KEY ([NewsFeedId], [NewsSectionReference], [SeqNo]),
    CONSTRAINT [FK_NewsFeedItems_NewsFeedSections_NewsFeedId_NewsSectionReference] FOREIGN KEY ([NewsFeedId], [NewsSectionReference]) REFERENCES [NewsFeedSections] ([NewsFeedId], [Reference]) ON DELETE CASCADE
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210819085221_Newsfeed', N'3.1.4');

GO

