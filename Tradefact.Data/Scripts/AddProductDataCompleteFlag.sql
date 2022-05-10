ALTER TABLE [Products] ADD [DataComplete] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211122152842_AddProductDataCompleteFlag', N'3.1.4');

GO

UPDATE [dbo].[Products]
SET [DataComplete] = CAST(1 AS bit)
WHERE Dimensions_Height IS NOT NULL
  AND Dimensions_Length IS NOT NULL
  AND Dimensions_Width IS NOT NULL
  AND Dimensions_Weight IS NOT NULL
  AND Packing IS NOT NULL
  AND UnitsPerPackage > 0.0;

GO

