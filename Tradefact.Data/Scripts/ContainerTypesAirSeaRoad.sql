
ALTER TABLE [ContainerTypes] ADD [Air] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [ContainerTypes] ADD [Road] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [ContainerTypes] ADD [Sea] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210422111528_ContainerTypesAirSeaRoad', N'3.1.4');

GO

UPDATE [dbo].[ContainerTypes] SET Sea = 1

INSERT INTO [dbo].[ContainerTypes] (Code, [Description], ISOTypeGroup, ISOTypeGroupDescription, [Length], Height, Width, AdditionalInformation, Active,	Air, Road, Sea)
SELECT '00R1', 'Refrigerated', ISOTypeGroup, ISOTypeGroupDescription, [Length], Height, Width, 'Refrigerated Road Transport', Active,	0, 1, 0
 FROM [dbo].[ContainerTypes] WHERE Code = '00G0'
UNION
SELECT '00R0', 'Dry', ISOTypeGroup, ISOTypeGroupDescription, [Length], Height, Width, 'Dry Road Transport', Active,	0, 1, 0
 FROM [dbo].[ContainerTypes] WHERE Code = '00G0'

UPDATE [dbo].[ContainerTypes] SET Active = 1 WHERE Road = 1
