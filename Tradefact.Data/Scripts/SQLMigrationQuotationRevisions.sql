BEGIN TRAN

    ALTER TABLE [Quotations] DROP CONSTRAINT [FK_Quotations_QuotationRequests_QuotationRequestId];
    GO

    DROP INDEX [IX_Quotations_QuotationRequestId] ON [Quotations];
    GO

    ALTER TABLE [Quotations] ADD [Revision] int NOT NULL DEFAULT 0;
    GO

    CREATE INDEX [IX_Quotations_QuotationRequestId_Id] ON [Quotations] ([QuotationRequestId], [Id]);
    GO

    ALTER TABLE [Quotations] ADD CONSTRAINT [FK_Quotations_QuotationRequests_QuotationRequestId] FOREIGN KEY ([QuotationRequestId]) REFERENCES [QuotationRequests] ([Id]) ON DELETE NO ACTION;
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201216225541_QuotationRevisions', N'3.1.4');

ROLLBACK TRAN

