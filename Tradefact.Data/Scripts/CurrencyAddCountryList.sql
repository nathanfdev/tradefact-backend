
ALTER TABLE [Currency] ADD [Symbol] nvarchar(8) NULL;

GO

CREATE TABLE [CurrencyCountries] (
    [CurrencyId] int NOT NULL,
    [CountryCode] nvarchar(4) NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_CurrencyCountries] PRIMARY KEY ([CurrencyId], [CountryCode]),
    CONSTRAINT [FK_CurrencyCountries_Countries_CountryCode] FOREIGN KEY ([CountryCode]) REFERENCES [Countries] ([Code2]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CurrencyCountries_Currency_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency] ([CurrencyId]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_CurrencyCountries_CountryCode] ON [CurrencyCountries] ([CountryCode]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210730135616_CurrencyAddCountryList', N'3.1.4');

GO

