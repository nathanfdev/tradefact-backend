ALTER TABLE [PurchaseOrders] ADD [IsLocked] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210518121534_PurchaseOrderAddedIsLocked', N'3.1.4');
GO

;with cte_po AS (
    SELECT PO.[Id], MAX(CASE WHEN Shipping = 1 OR Completed = 1 THEN 1 ELSE 0 END) [Locked] , COUNT(SH.Id) [Schedules]
        FROM [dbo].[PurchaseOrders] PO 
        LEFT JOIN [dbo].[PurchaseOrderItemScheduleLines] SH ON SH.PurchaseOrderId = PO.Id
    GROUP BY PO.[id]
) 
UPDATE P SET P.IsLocked = 1
FROM cte_po CP
    INNER JOIN [dbo].[PurchaseOrders] P ON P.Id = CP.Id 
WHERE Locked > 0 OR Schedules > 0

