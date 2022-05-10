--DECLARE @var0 sysname;
--SELECT @var0 = [d].[name]
--FROM [sys].[default_constraints] [d]
--INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
--WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoItems]') AND [c].[name] = N'Discriminator');
--IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CargoItems] DROP CONSTRAINT [' + @var0 + '];');
--ALTER TABLE [CargoItems] DROP COLUMN [Discriminator];

--GO

ALTER TABLE [Organisations] ADD [ChargebeeSubscriptionId] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210427112406_ActivationChargebee', N'3.1.4');

GO

