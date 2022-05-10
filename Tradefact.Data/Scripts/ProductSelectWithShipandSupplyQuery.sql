declare @OrganisationId UNIQUEIDENTIFIER = '92839fb2-cd9b-441c-9619-50e9d64bb899';
declare @PageSize int = 122;
declare @PageNumber int = 1;

DECLARE @Products TABLE
(   
    ProductId UNIQUEIDENTIFIER NOT NULL, 
    NoPurchaseOrders INT DEFAULT 0, 
    OrderQty INT DEFAULT 0, 
    NoShipments INT DEFAULT 0, 
    ShipQty INT DEFAULT 0, 
    ShipQtyInTransit INT DEFAULT 0, 
    ShipNoCartons INT DEFAULT 0,
    ShipNoCartonsInTransit	 INT DEFAULT 0
)

;WITH cte_po_totals AS (

    SELECT PO.ProductId, COUNT(PO.PurchaseOrderId) [NoPurchaseOrders], SUM(Qty) [OrderQty] FROM (
        SELECT PI.ProductId, P.Id [PurchaseOrderId], SUM(PI.OrderQuantity) [Qty]
        FROM [dbo].[PurchaseOrders] P
            INNER JOIN [dbo].PurchaseOrderItems PI ON PI.PurchaseOrderId = P.Id
        WHERE P.CompanyId = @OrganisationId AND P.Active = 1 AND (P.[Status] >= 20 and P.[Status] < 70)
        GROUP BY PI.ProductId, P.Id
    ) PO
    GROUP BY PO.ProductId

), cte_ship_totals AS (

    SELECT  S.ProductId, COUNT(S.Id) [NoShipments], 
            SUM(ShipQty) [ShipQty], 
            SUM(ShipNoCartons) [ShipNoCartons],
            SUM(ShipQty) [ShipQtyInTransit], 
            SUM(ShipNoCartons) [ShipNoCartonsInTransit] 
    FROM (
        SELECT S.Id, CI.ProductId, 
               SUM(CI.CartonQty) [ShipNoCartons], SUM(CI.Qty) [ShipQty],
               SUM(CASE WHEN S.Collected= 1 AND S.ArrivedPOD = 0 THEN CI.CartonQty ELSE 0 END) [CartonsInTransit],
               SUM(CASE WHEN S.Collected= 1 AND S.ArrivedPOD = 0 THEN CI.Qty ELSE 0 END) [QtyInTransit]
        FROM [dbo].[Partnerships] P
            INNER JOIN [dbo].Shipments S ON S.PartnershipId = P.Id
            LEFT JOIN [dbo].CargoItems CI ON CI.FreightMovementId = S.FreightMovementId
        WHERE P.ClientId = @OrganisationId AND S.Active = 1
        GROUP BY S.Id, CI.ProductId
    ) S
    GROUP BY S.ProductId
)
INSERT INTO @Products (ProductId, NoPurchaseOrders, OrderQty, NoShipments, ShipQty, ShipQtyInTransit, ShipNoCartons, ShipNoCartonsInTransit)
SELECT P.Id, ISNULL(PUR.NoPurchaseOrders,0), ISNULL(PUR.OrderQty,0), ISNULL(SHP.NoShipments,0), ISNULL(SHP.ShipQty,0), ISNULL(SHP.ShipQtyInTransit,0), ISNULL(SHP.ShipNoCartons,0), ISNULL(SHP.ShipNoCartonsInTransit,0)
 FROM [dbo].[Products] P
    LEFT JOIN cte_po_totals PUR ON PUR.ProductId = P.Id
    LEFT JOIN cte_ship_totals SHP ON SHP.ProductId = P.Id
WHERE P.CompanyId = @OrganisationId
ORDER BY SHP.ShipQty desc, P.Name
OFFSET (@PageNumber-1)*@PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY

SELECT P.*, 
        ISNULL(Ps.NoShipments,0) [ActiveOrders], ISNULL(Ps.ShipNoCartons, 0) [CartonQtyOnOrder], ISNULL(Ps.ShipNoCartonsInTransit, 0) [CartonQtyInTransit], 
        (ISNULL(Ps.ShipNoCartons, 0) * P.UnitsPerPackage) [UnitsonOrder], (ISNULL(Ps.ShipNoCartonsInTransit, 0) * P.UnitsPerPackage) [UnitsInTransit]
FROM @Products PS
    INNER JOIN dbo.Products P ON P.Id = PS.ProductId
    ORDER BY ShipQty desc, P.Name

SELECT PS.ProductId, PS.SupplierId, PSS.Name [SupplierName], PS.SupplierReference, PS.Price, PS.OrderQuantityMinimum
FROM @Products P
    INNER JOIN [dbo].[ProductSuppliers] PS ON PS.ProductId = P.ProductId and PS.Active = 1
    LEFT JOIN [dbo].[Organisations] PSS ON PSS.Id = PS.SupplierId

SELECT COUNT(*)
    FROM [dbo].Products P
WHERE P.CompanyId = @organisationId AND P.Active = 1;