CREATE TABLE [Airlines] (
    [IATA2LetterCcode] nvarchar(2) NOT NULL,
    [AWBPrefix] nvarchar(3) NOT NULL,
    [Name] nvarchar(256) NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Tracking_Enabled] bit NULL DEFAULT CAST(0 AS bit),
    [Tracking_Provider] nvarchar(32) NULL,
    CONSTRAINT [PK_Airlines] PRIMARY KEY ([AWBPrefix], [IATA2LetterCcode])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210114230638_AWBTrackingAddedAirlines', N'3.1.4');

GO

