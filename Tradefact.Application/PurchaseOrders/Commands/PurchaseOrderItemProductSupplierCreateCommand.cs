using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderItemProductSupplierCreateCommand : IRequest<Guid>
    {
        public Guid PurchaseOrderId { get; set; }
        public string LastChangeUser { get; set; }

        public class PurchaseOrderItemProductSupplierCreateCommandHandler : IRequestHandler<PurchaseOrderItemProductSupplierCreateCommand, Guid>
        {
            private readonly TradefactDbContext _context;
            private readonly IDbConnection _connection;

            public PurchaseOrderItemProductSupplierCreateCommandHandler(TradefactDbContext context, IDbConnection connection)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _connection = connection;
            }

            public async Task<Guid> Handle(PurchaseOrderItemProductSupplierCreateCommand cmd, System.Threading.CancellationToken cancellationToken)
            {
                string sQuery = @"
                    DECLARE @LastModifiedTime DATETIME2 = GETDATE();

                    BEGIN TRY
    
                        BEGIN TRAN

                        -- INSERT/UPDATE ON [dbo].ProductSupplier
                        ;WITH source_po_products AS (
                            SELECT DISTINCT PO.SupplierId, POI.ProductId, @LastChangeUser [LastChangeUser], @LastModifiedTime [LastModifiedTime]
                            FROM [dbo].PurchaseOrders PO
                                INNER JOIN [dbo].PurchaseOrderItems POI ON POI.PurchaseOrderId = PO.Id
                            WHERE PO.Id = @PurchaseOrderId
                        )
                        MERGE [dbo].ProductSuppliers AS [target]
                        USING source_po_products AS [source]  
                        ON ([target].SupplierId = [source].SupplierId AND [target].ProductId = [source].ProductId AND [target].Active = 1)
                        WHEN MATCHED THEN     
                            UPDATE SET  [LastChangeUser] = [source].[LastChangeUser]
                                        ,[LastModifiedOnInternal] = [source].LastModifiedTime
                            WHEN NOT MATCHED THEN
                            INSERT 
                               (Id ,[ProductId] ,[SupplierId] ,[Active] ,[SupplierReference] ,[Price] ,[OldPrice] ,[CreatedByUser] ,[CreationDateInternal] ,[LastChangeUser] ,[LastModifiedOnInternal] ,[OrderQuantityMinimum] ,[Currency])
                         VALUES
                               (NEWID() ,[source].ProductId ,[source].SupplierId ,1 ,null ,0 ,0 ,@LastChangeUser ,@LastModifiedTime ,@LastChangeUser ,@LastModifiedTime ,0 ,NULL);

                        -- INSERT/UPDATE ON [dbo].ProductSupplierCurrency
                        ;WITH source_po_product_suppliers AS (
                            SELECT PS.Id [ProductSupplierId], PL.* FROM (
                                SELECT  PO.SupplierId, POI.ProductId, 
                                        PO.CurrencyId [Currency], MAX(ISNULL(POI.OrderPriceUnit,0)) [Price],
                                        @LastChangeUser [LastChangeUser], @LastModifiedTime [LastModifiedTime]
                                FROM [dbo].PurchaseOrders PO
                                    INNER JOIN [dbo].PurchaseOrderItems POI ON POI.PurchaseOrderId = PO.Id
                                WHERE PO.Id = @PurchaseOrderId
                                GROUP BY PO.SupplierId, POI.ProductId, PO.CurrencyId
                            ) PL
                            INNER JOIN [dbo].[ProductSuppliers] PS ON PS.ProductId = PL.ProductId AND PS.SupplierId = PL.SupplierId AND PS.Active = 1
                        )
                        MERGE [dbo].ProductSupplierCurrency AS [target]
                        USING source_po_product_suppliers AS [source]  
                        ON ([target].ProductSupplierId = [source].[ProductSupplierId] AND [target].CurrencyCode = [source].Currency AND [target].IsActive = 1)
                        WHEN MATCHED THEN     
                            UPDATE SET   [Price] = [source].Price
                                        ,[LastChangeUser] = [source].[LastChangeUser]
                                        ,[LastModifiedOnInternal] = [source].LastModifiedTime
                            WHEN NOT MATCHED THEN
                            INSERT 
                               ([Id],[IsActive],[CreatedByUser],[CreationDateInternal],[LastChangeUser],[LastModifiedOnInternal],[LastModifiedOn],[CreationDate],[ProductSupplierId],[CurrencyCode],[Price])
                         VALUES
                               (NEWID(), 1, @LastChangeUser ,@LastModifiedTime ,@LastChangeUser ,@LastModifiedTime, @LastModifiedTime, @LastModifiedTime, [source].[ProductSupplierId],[source].Currency, ISNULL([source].Price, 0));

                        COMMIT TRAN -- Transaction Success!

                    END TRY
                    BEGIN CATCH

                        IF @@TRANCOUNT > 0
                            ROLLBACK TRAN --RollBack in case of Error

                        DECLARE @ErrorMessage NVARCHAR(4000);
                        DECLARE @ErrorSeverity INT;
                        DECLARE @ErrorState INT;

                        SELECT 
                            @ErrorMessage = ERROR_MESSAGE(),
                            @ErrorSeverity = ERROR_SEVERITY(),
                            @ErrorState = ERROR_STATE();

                        RAISERROR (@ErrorMessage, -- Message text.
                                    @ErrorSeverity, -- Severity.
                                    @ErrorState -- State.
                                    );
                    END CATCH
                    ";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    await conn.QueryAsync(sQuery, new
                    {
                        @purchaseOrderId = cmd.PurchaseOrderId,
                        @lastChangeUser = cmd.LastChangeUser
                    });

                }
                _ = await _context.SaveChangesAsync();

                return cmd.PurchaseOrderId;
            }

        }
    }
}
