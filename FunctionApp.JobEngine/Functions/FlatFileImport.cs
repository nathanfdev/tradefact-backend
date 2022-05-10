using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Models;
using Core.ServiceBus;
using Dapper;
//using Core.Models;
using FunctionApp.JobEngine.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp.JobEngine.Functions
{
    public static class FlatFileImport
    {
        private static HttpClient _httpClient = GetFlatfileHttpClient();

        private static HttpClient GetFlatfileHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", "FF00SFS58DQTY1GJE233AXPKW3ZW3O9M3P7ND04W+qPR79PLmykbsjx95J8xgLRDsEApJccDLYxfv3JXX");
            return client;
        }

        [FunctionName(nameof(FlatFilePOImport))]
        public static async Task FlatFilePOImport([ServiceBusTrigger("flatfilepoimport", Connection = "connectionstring")] string message,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            ServiceBusMessage<PurchaseOrderFlatfileImport> payload = JsonConvert.DeserializeObject<ServiceBusMessage<PurchaseOrderFlatfileImport>>(message);
            try
            {
                List<ImportedPOItem> data = await RetrieveFlatfileBatch(payload.Body, log);
                _ = await UploadBatchSQL(payload.Body, data, log);
            }
            catch (Exception ex)
            {
                log.LogInformation($"Error Batch: {payload.CorrelationId} {ex.Message}");

                var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

                using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
                {
                    connection.Open();

                    var error_result = connection.Execute("UPDATE Q SET Q.[Status] = 99, Q.[Result] = Q.[Description] + ' FAILED' FROM [dbo].[QueuedTask] Q WHERE Q.[Id] = @correlationId", new { @correlationId = payload.CorrelationId });

                    connection.Close();
                }
            }

        }

        [FunctionName(nameof(ExternalFlatFilePOImport))]
        public static async Task ExternalFlatFilePOImport([ServiceBusTrigger("externalflatfilepoimport", Connection = "connectionstring")] string message,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            ServiceBusMessage<PurchaseOrderFlatfileImport> payload = JsonConvert.DeserializeObject<ServiceBusMessage<PurchaseOrderFlatfileImport>>(message);
            try
            {
                List<ImportedPOItem> data = await RetrieveFlatfileBatch(payload.Body, log);

                _ = await UploadExternalPOItemsBatchSQL(payload.Body, data, log);

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error Batch: {payload.CorrelationId} {ex.Message}");

                var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

                using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
                {
                    connection.Open();

                    var error_result = connection.Execute("UPDATE Q SET Q.[Status] = 99, Q.[Result] = Q.[Description] + ' FAILED' FROM [dbo].[QueuedTask] Q WHERE Q.[Id] = @correlationId", new { @correlationId = payload.CorrelationId });

                    connection.Close();
                }
            }

        }

        [FunctionName(nameof(FlatFileProductImport))]
        public static async Task FlatFileProductImport([ServiceBusTrigger("flatfileproductimport", Connection = "connectionstring")] string message,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            ServiceBusMessage<ProductFlatfileImport> payload = JsonConvert.DeserializeObject<ServiceBusMessage<ProductFlatfileImport>>(message);
            try
            {
                if (!payload.Body.CompanyId.HasValue) payload.Body.CompanyId = Guid.Parse("92839fb2-cd9b-441c-9619-50e9d64bb899");

                List<ImportedProductItem> data = await RetrieveFlatfileProductBatch(payload.Body, log);
                _ = await UploadProductBatchSQL(payload.Body, data, log);
            }
            catch (Exception ex)
            {
                log.LogInformation($"Error Batch: {payload.CorrelationId} {ex.Message}");

                var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

                using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
                {
                    connection.Open();

                    var error_result = connection.Execute("UPDATE Q SET Q.[Status] = 99, Q.[Result] = Q.[Description] + ' FAILED' FROM [dbo].[QueuedTask] Q WHERE Q.[Id] = @correlationId", new { @correlationId = payload.CorrelationId });

                    connection.Close();
                }
            }

        }

        private static PurchaseOrderFlatfileImport DeserializeQueueMessage(string queueMessage)
        {
            return JsonConvert.DeserializeObject<PurchaseOrderFlatfileImport>(queueMessage);
        }

        private static async Task<bool> UploadBatchSQL(PurchaseOrderFlatfileImport payload, List<ImportedPOItem> data, ILogger log)
        {
            var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            DataTable dt = new DataTable();
            dt.Columns.Add("CorrelationId", typeof(Guid));
            dt.Columns.Add("Seq", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("PurchasePrice", typeof(decimal));
            dt.Columns.Add("SalesPrice", typeof(decimal));

            foreach (var item in data.Take(10000))
            {
                dt.Rows.Add(payload.CorrelationId, item.Seq, item.Description, item.SKU, item.Quantity, item.PurchasePrice, item.SalesPrice);
            }

            log.LogInformation($"Uploading Batch: {payload.CorrelationId}");


            string insert_sql = @"

            DECLARE @actionTime DATETIME = GETDATE();

            DECLARE @inserted_rows int = 0

            BEGIN TRY
                BEGIN TRAN

                INSERT INTO [dbo].[PurchaseOrderItems]
                        ( [PurchaseOrderId] ,[Id] ,[Active] ,[ProductId] ,[SKU] ,[PurchaseOrderItemText] ,[OrderQuantity] ,[OrderQuantityUnit]
                            ,[OrderPriceUnit] ,[NetPriceAmount] ,[NetPriceQuantity] ,[TaxCode] ,[TaxCountry] ,[TaxJurisdiction] ,[TaxDeterminationDate]
                            ,[IsDeliveryComplete] ,[IsFinallyInvoiced] ,[PurchaseOrderItemCategory] ,[AccountAssignmentCategory] ,[PurchaseContract]
                            ,[ItemNetWeight] ,[ItemWeightUnit] ,[ItemVolume] ,[ItemVolumeUnit] ,[CreatedByUser] ,[CreationDateInternal] ,[LastChangeUser] ,[LastModifiedOnInternal], [IsBulkUpload])
                SELECT  @purchaseOrderId ,NEWID() ,1 ,@productId ,SKU ,LEFT([Description], 64) ,Quantity ,null ,PurchasePrice ,0 ,0 ,null ,null ,null ,null
                        ,0 ,0 ,null ,null ,null ,0 ,null ,0 ,null ,@userId ,DATEADD(ms, (seq*100), @actionTime) ,@userId ,DATEADD(ms, (seq*100), @actionTime), 1
                FROM [dbo].[FlatFileSimplePO] where [CorrelationId] = @correlationId

                SELECT @inserted_rows=@@ROWCOUNT

                DELETE FROM [dbo].[FlatFileSimplePO] where [CorrelationId] = @correlationId

                UPDATE Q SET Q.[Status] = 2, Q.[Result] = Q.[Description] + CONVERT(VARCHAR(36), @inserted_rows) + ' row items inserted' FROM [dbo].[QueuedTask] Q WHERE Q.[Id] = @correlationId

                declare @TaxRate decimal(9,2) = 0;
                declare @TaxRateM decimal(9,2) = 0

                SELECT @TaxRate = ISNULL(SUM(Rate),0) FROM [dbo].[PurchaseOrderAdditionalCharges] AC 
                WHERE AC.PurchaseOrderId = @PurchaseOrderId and AC.[Type]=1 and AC.IsActive=1;;

                SELECT @TaxRateM = @TaxRate / 100.00

                ;WITH cte_line_values AS (
                    SELECT POI.PurchaseOrderId, POI.Id [ItemId], 'I' [Type], [OrderQuantity] [LI_OrderQuantity], 
                    ISNULL([OrderPriceUnit], 0) [LI_OrderPriceUnit], 
                    (ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0)) [LI_NetAmount], 
                    ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0))*@TaxRateM) [LI_Vat],
                    ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0)) + ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0))*@TaxRateM)) [LI_TotalAmount],
                    0 [CI_OrderQuantity], 0 [CI_OrderPriceUnit], 0 [CI_NetAmount], 0 [CI_Vat], 0 [CI_TotalAmount], 1 [Lines]
                    FROM [dbo].[PurchaseOrderItems] POI 
                    WHERE POI.PurchaseOrderId = @PurchaseOrderId and POI.[Active] = 1
                    UNION
                    SELECT POC.PurchaseOrderId, POC.Id [ItemId], 'C' [Type],
                    0 [LI_OrderQuantity], 0 [LI_OrderPriceUnit], 0 [LI_NetAmount], 0 [LI_Vat], 0 [LI_TotalAmount], 
                    1 [CI_OrderQuantity], 
                    ISNULL(Rate,0) [CI_OrderPriceUnit], 
                    1 * ISNULL(Rate,0) [CI_NetAmount], 
                    (1 * ISNULL(Rate,0))*(0) [CI_Vat], 
                    ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(0))) [CI_TotalAmount], 0 [Lines] 
                -- (1 * ISNULL(Rate,0))*(@TaxRateM) [Vat],
                -- ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(@TaxRateM))) [TotalAmount]
                    FROM [dbo].[PurchaseOrderAdditionalCharges] POC 
                    WHERE POC.PurchaseOrderId = @PurchaseOrderId and POC.[IsActive] = 1 and POC.[Type] = 2
                ),
                cte_order_totals AS (
                    SELECT PurchaseOrderId, 
                    SUM(LI_NetAmount) [I_NetAmount], 
                    SUM(LI_Vat) [I_Vat], 
                    SUM(LI_TotalAmount) [I_TotalAmount], 
                    SUM(Lines) [NoOfItems], 

                    SUM(CI_NetAmount) [C_NetAmount], 
                    SUM(CI_Vat) [C_Vat], 
                    SUM(CI_TotalAmount) [C_TotalAmount],

                    SUM(LI_NetAmount) + SUM(CI_NetAmount) [NetAmount], 
                    SUM(LI_Vat) + SUM(CI_Vat) [Vat], 
                    SUM(LI_TotalAmount) + SUM(CI_TotalAmount) [TotalAmount]  

                    FROM cte_line_values
                    GROUP BY PurchaseOrderId
                )
                UPDATE P
                    SET P.TaxRate = @TaxRate, 
                        P.BaseCurrency_NetAmount = T.NetAmount, P.BaseCurrency_TaxAmount = T.Vat, P.BaseCurrency_TotalAmount = T.TotalAmount,
                        P.ItemsTotal_NetAmount = T.I_NetAmount, ItemsTotal_TaxAmount = T.I_Vat, ItemsTotal_TotalAmount = T.I_TotalAmount,
                        P.ChargesTotal_NetAmount = T.C_NetAmount, ChargesTotal_TaxAmount = T.C_Vat, ChargesTotal_TotalAmount = T.C_TotalAmount,
                        P.Total_NetAmount = T.NetAmount, Total_TaxAmount = T.Vat, Total_TotalAmount = T.TotalAmount, P.NumberOfItems = T.NoOfItems
                FROM [dbo].[PurchaseOrders] P
                    INNER JOIN cte_order_totals T ON T.PurchaseOrderId = P.Id
                WHERE P.Id = @PurchaseOrderId


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


            using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
            {
                connection.Open();


                var cleanup_result = connection.Execute("DELETE FROM [dbo].[FlatFileSimplePO] where [CorrelationId] = @correlationId", new { @correlationId = payload.CorrelationId });


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "[dbo].[FlatFileSimplePO]";
                    bulkCopy.WriteToServer(dt);
                }

                var insert_result = connection.Execute(insert_sql, new {
                    @correlationId = payload.CorrelationId,
                    @purchaseOrderId = payload.PurchaseOrderId,
                    @productId = payload.ProductId,
                    @userId = payload.UserId
                });

                connection.Close();
            }

            return true;
        }

        private static string GeneratePONumber()
        {
            DateTime _now = DateTime.Now;

            StringBuilder builder = new StringBuilder();
            builder.Append("Order");
            builder.Append("-");
            builder.Append(_now.ToString("yy"));
            builder.Append("-");
            builder.Append(_now.ToString("MM"));
            builder.Append("-");

            Random random = new Random();
            char ch;
            for (int i = 0; i < 5; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        private static async Task<bool> UploadExternalPOItemsBatchSQL(PurchaseOrderFlatfileImport payload, List<ImportedPOItem> data, ILogger log)
        {
            var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            string purchaseOrderNumber = String.IsNullOrEmpty(payload.PurchaseOrderNo) ? GeneratePONumber() : payload.PurchaseOrderNo;

            DataTable dt = new DataTable();
            dt.Columns.Add("CorrelationId", typeof(Guid));
            dt.Columns.Add("Seq", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("PurchasePrice", typeof(decimal));
            dt.Columns.Add("SalesPrice", typeof(decimal));

            foreach (var item in data.Take(10000))
            {
                dt.Rows.Add(payload.CorrelationId, item.Seq, item.Description, item.SKU, item.Quantity, item.PurchasePrice, item.SalesPrice);
            }

            log.LogInformation($"Uploading Batch: {payload.CorrelationId}");


            string insert_sql = @"

            DECLARE @actionTime DATETIME = GETDATE();

            DECLARE @inserted_rows int = 0

            DECLARE @inserted_id UNIQUEIDENTIFIER  = NEWID();

            BEGIN TRY
                BEGIN TRAN

                INSERT INTO [integration].[ExternalPurchaseOrders]
                        ([Id], [Active], [ExternalID], [PurchaseOrderNumber], [Reference]
                        ,[OrderDate], [GoodsReadyDate], [DateOfIssue], [PlaceOfIssue], [Status]
                        ,[CurrencyRate], [CurrencyCode], [SubTotal], [TotalTax], [Total]
                        ,[PaymentTerms], [SupplierName], [Tags], [Received], [Imported]
                        ,[ImportDate], [ImportUserId], [GenericProductId], [CreatedByUser], [CreationDateInternal]
                        ,[LastChangeUser], [LastModifiedOnInternal], [OrganisationId], [SupplierId], [Source])
                SELECT TOP 1 
                        @inserted_id, 1, NEWID(), @purchaseOrderNumber, null
                        ,@actionTime, @actionTime, @actionTime, null, null
                        ,0, @currencyCode, 0, 0, 0
                        ,null, O.Name, null, @actionTime, 0
                        ,null, null, null, @userId, @actionTime
                        ,null, @actionTime, @organisationId, @supplierId, 'CSV'
                FROM [dbo].[Organisations] O WHERE O.Id = @supplierId

                INSERT INTO [integration].[ExternalPurchaseOrderLineItems]
                    ([Id], [ExternalPurchaseOrderId], [Active], [LineItemID], [SKU]
                    ,[Description], [SupplierReference], [Quantity], [UnitPrice], [TaxType]
                    , [TaxAmount], [LineAmount], [CreatedByUser], [CreationDateInternal], [LastChangeUser], [LastModifiedOnInternal])
                SELECT
                    NEWID(), @inserted_id, 1, NEWID(), [SKU]
                    ,[Description], null, [Quantity], [PurchasePrice], null
                    ,0, [PurchasePrice], @userId, DATEADD(ms, (seq*100), @actionTime), @userId, DATEADD(ms, (seq*100), @actionTime)
                FROM [dbo].[FlatFileSimplePO] where [CorrelationId] = @correlationId

                SELECT @inserted_rows=@@ROWCOUNT

                DELETE FROM [dbo].[FlatFileSimplePO] where [CorrelationId] = @correlationId

                UPDATE Q SET Q.[Status] = 2, Q.[Result] = Q.[Description] + CONVERT(VARCHAR(36), @inserted_rows) + ' row items inserted' FROM [dbo].[QueuedTask] Q WHERE Q.[Id] = @correlationId

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


            using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
            {
                connection.Open();


                var cleanup_result = connection.Execute("DELETE FROM [dbo].[FlatFileSimplePO] where [CorrelationId] = @correlationId", new { @correlationId = payload.CorrelationId });


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "[dbo].[FlatFileSimplePO]";
                    bulkCopy.WriteToServer(dt);
                }

                var insert_result = connection.Execute(insert_sql, new
                {
                    @correlationId = payload.CorrelationId,
                    @purchaseOrderId = payload.PurchaseOrderId,
                    @productId = payload.ProductId,
                    @userId = payload.UserId,
                    @supplierId = payload.SupplierId,
                    @purchaseOrderNumber = purchaseOrderNumber,
                    @currencyCode = payload.CurrencyCode,
                    @organisationId = payload.OrganisationId
                });

                connection.Close();
            }

            return true;
        }


        private static async Task<bool> UploadProductBatchSQL(ProductFlatfileImport payload, List<ImportedProductItem> data, ILogger log)
        {
            var sqldb_connection_string = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);

            DataTable dt = new DataTable();
            dt.Columns.Add("CorrelationId", typeof(Guid));
            dt.Columns.Add("Seq", typeof(int));
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("HsCode", typeof(string));
            dt.Columns.Add("StockQuantity", typeof(decimal));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("CompanyId", typeof(Guid));
            dt.Columns.Add("ProductId", typeof(Guid));
            dt.Columns.Add("SupplierId", typeof(Guid));
            dt.Columns.Add("Description", typeof(string));

            foreach (var item in data.Take(10000))
            {
                dt.Rows.Add(payload.CorrelationId, item.Seq, item.SKU, item.Name ?? "", item.HsCode ?? "", item.StockQuantity, item.UnitPrice, 
                    payload.CompanyId, 
                    Guid.NewGuid(), 
                    payload.SupplierId.HasValue ? payload.SupplierId.GetValueOrDefault() : null, item.Description ?? "");
            }

            log.LogInformation($"Uploading Batch: {payload.CorrelationId}");


            string insert_sql = @"

            DECLARE @actionTime DATETIME = GETDATE();

            DECLARE @inserted_rows int = 0

            BEGIN TRY
                BEGIN TRAN

                INSERT INTO [dbo].[Products]
                        ( Id, CompanyId, Active, SKU, Name, Description, HsCode,
                          MinStockQuantity, NotifyStockQuantityBelow, OrderQuantityMaximum, StockQuantity,
                          MagneticFieldContained, UnitsPerPackage, HazardousContents, Rotatable, Stackable,IncomingStockQuantity,
                          [CreatedByUser] ,[CreationDateInternal] ,[LastChangeUser] ,[LastModifiedOnInternal])
                SELECT  [ProductId], [CompanyId] ,1 ,SKU ,LEFT([Name], 250) ,LEFT([Description], 250) ,LEFT([HsCode], 64),
                        0 ,0 ,0,[StockQuantity],
                        0,	0,	0,	0,	0,	0,
                        @userId ,DATEADD(ms, (seq*100), @actionTime) ,@userId ,DATEADD(ms, (seq*100), @actionTime)
                FROM [dbo].[FlatFileProductImport] where [CorrelationId] = @correlationId

                SELECT @inserted_rows=@@ROWCOUNT

                INSERT INTO [dbo].[ProductSuppliers] (
                    [Id], [ProductId], [SupplierId], [Active], [SupplierReference],
                    [Price], [OldPrice], [OrderQuantityMinimum],
                    [CreatedByUser] ,[CreationDateInternal] ,[LastChangeUser] ,[LastModifiedOnInternal], Currency
                )
                SELECT  NEWID(), [ProductId], [SupplierId] ,1 ,NULL,
                        UnitPrice, UnitPrice, 1,
                        @userId ,DATEADD(ms, (seq*100), @actionTime) ,@userId ,DATEADD(ms, (seq*100), @actionTime), ISNULL(O.Currency, 'USD')
                FROM [dbo].[FlatFileProductImport] FFI
                    LEFT JOIN [dbo].[Organisations] O ON O.Id = FFI.SupplierId
                where [CorrelationId] = @correlationId AND [SupplierId] IS NOT NULL


                DELETE FROM [dbo].[FlatFileProductImport] where [CorrelationId] = @correlationId

                UPDATE Q SET Q.[Status] = 2, Q.[Result] = Q.[Description] + CONVERT(VARCHAR(36), @inserted_rows) + ' row items inserted' FROM [dbo].[QueuedTask] Q WHERE Q.[Id] = @correlationId

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


            using (SqlConnection connection = new SqlConnection(sqldb_connection_string))
            {
                connection.Open();


                var cleanup_result = connection.Execute("DELETE FROM [dbo].[FlatFileProductImport] where [CorrelationId] = @correlationId", new { @correlationId = payload.CorrelationId });


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "[dbo].[FlatFileProductImport]";
                    bulkCopy.WriteToServer(dt);
                }

                var insert_result = connection.Execute(insert_sql, new
                {
                    @correlationId = payload.CorrelationId,
                    @userId = payload.UserId
                });

                connection.Close();
            }

            return true;
        }


        private static async Task<List<ImportedPOItem>> RetrieveFlatfileBatch(PurchaseOrderFlatfileImport request, ILogger log)
        {
            log.LogInformation($"Flatfile Fetching Batch: {request.BatchId}.");

            List<ImportedPOItem> order_items = new List<ImportedPOItem>();

            HttpResponseMessage response = await _httpClient.GetAsync($"https://api.us.flatfile.io/rest/batch/{request.BatchId}/rows?take=100000");
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"Flatfile Batch: {request.BatchId} - Response {response.StatusCode}.");
                var responseContent = (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent) ? await response.Content.ReadAsStringAsync() : null;
                if (responseContent != null)
                {
                    FlatFileBatch result = JsonConvert.DeserializeObject<FlatFileBatch>(responseContent);
                    int noOfItems = result.pagination.totalCount;
                    int seq = 1;
                    foreach (var item in result.data)
                    {
                        ImportedPOItem i = new ImportedPOItem();

                        List<string> descriptions = new List<string>() { item.mapped.desc1, item.mapped.desc2, item.mapped.desc3, item.mapped.desc4, item.mapped.desc5 };
                        i.Description = String.Join(" ", descriptions.Where(s => !string.IsNullOrEmpty(s)));


                        i.Quantity = 0;
                        if (!String.IsNullOrEmpty(item.mapped.qty))
                        {
                            string qty_cleaned = Regex.Match(item.mapped.qty, @"[-+]?\b[0-9]+(\.[0-9]+)?").Value;
                            if (Decimal.TryParse(qty_cleaned, out decimal qty))
                            {
                                i.Quantity = qty;
                            }
                        }

                        i.PurchasePrice = 0;
                        if (!String.IsNullOrEmpty(item.mapped.unitprice))
                        {
                            string price_cleaned = Regex.Match(item.mapped.unitprice, @"[-+]?\b[0-9]+(\.[0-9]+)?").Value;
                            if (Decimal.TryParse(price_cleaned, out decimal price))
                            {
                                i.PurchasePrice = price;
                            }
                        }

                        i.SKU = item.mapped.sku;
                        i.Seq = seq;
                        order_items.Add(i);
                        seq += 1;
                    }
                }
            }
            else
            {
                string msg = $"Flatfile Batch: {request.BatchId} - Error {response.StatusCode}.";
                log.LogError(msg);
                throw new Exception(msg);
            }
            return order_items;
        }
        private static async Task<List<ImportedProductItem>> RetrieveFlatfileProductBatch(ProductFlatfileImport request, ILogger log)
        {
            log.LogInformation($"Flatfile Fetching Batch: {request.BatchId}.");

            List<ImportedProductItem> order_items = new List<ImportedProductItem>();

            HttpResponseMessage response = await _httpClient.GetAsync($"https://api.us.flatfile.io/rest/batch/{request.BatchId}/rows?take=100000");
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"Flatfile Batch: {request.BatchId} - Response {response.StatusCode}.");
                var responseContent = (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent) ? await response.Content.ReadAsStringAsync() : null;
                if (responseContent != null)
                {
                    FlatFileBatch result = JsonConvert.DeserializeObject<FlatFileBatch>(responseContent);
                    int noOfItems = result.pagination.totalCount;
                    int seq = 1;
                    foreach (var item in result.data)
                    {
                        ImportedProductItem i = new ImportedProductItem();

                        i.Name = item.mapped.name;
                        i.Description = item.mapped.description;
                        i.HsCode = item.mapped.hscode;


                        i.StockQuantity = 0;
                        if (!String.IsNullOrEmpty(item.mapped.stockqty))
                        {
                            string qty_cleaned = Regex.Match(item.mapped.stockqty, @"\b\d+\b").Value;
                            if (Int32.TryParse(qty_cleaned, out int qty))
                            {
                                i.StockQuantity = qty;
                            }
                        }

                        i.UnitPrice = 0;
                        if (!String.IsNullOrEmpty(item.mapped.unitprice))
                        {
                            string price_cleaned = Regex.Match(item.mapped.unitprice, @"[-+]?\b[0-9]+(\.[0-9]+)?").Value;
                            if (Decimal.TryParse(price_cleaned, out decimal price))
                            {
                                i.UnitPrice = price;
                            }
                        }

                        i.SKU = item.mapped.sku;
                        i.Seq = seq;
                        i.CompanyId = request.CompanyId.GetValueOrDefault();

                        order_items.Add(i);
                        seq += 1;
                    }
                }
            }
            else
            {
                string msg = $"Flatfile Batch: {request.BatchId} - Error {response.StatusCode}.";
                log.LogError(msg);
                throw new Exception(msg);
            }
            return order_items;
        }

    }
}
