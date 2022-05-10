ALTER TABLE [Quotations] ADD [LoadType] int NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210610152934_AddedLoadtypetoQuotation', N'3.1.4');

GO

