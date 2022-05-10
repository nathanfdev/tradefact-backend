using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace FunctionApp.Testing
{
    public class Function1
    {
        private static HttpClient httpClient = new HttpClient();

        private static string qActivateAccount = Environment.GetEnvironmentVariable("Queue_AccountActivate", EnvironmentVariableTarget.Process);
        private static string TradefactDBConnectionString = Environment.GetEnvironmentVariable("TradefactDB", EnvironmentVariableTarget.Process);

        [FunctionName(nameof(SendPOEmailOrchestrator))]
        public async Task<bool> SendPOEmailOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, string payload)
        {
            PurchaseOrderEmailSendRequest request = await context.CallActivityAsync<PurchaseOrderEmailSendRequest>(nameof(DeserializeQueueMessage), payload);
            // Get purchase Order Information from database
            PurchaseOrderInfo po = await context.CallActivityAsync<PurchaseOrderInfo>(nameof(LoadPurchaseOrder), request.PurchaseOrderId);
            //if (po != null)
            //{
            //    // Generate PDF
            //    byte[] attachment = await context.CallActivityAsync<byte[]>(nameof(GeneratePurchaseOrderPDF), po);
            //}
            //else
            //{
            //    return false;
            //}

            // Do chargebee integrations


            // Replace "hello" with the name of your Durable Activity Function.

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return true;
        }


        [FunctionName(nameof(DeserializeQueueMessage))]
        public static PurchaseOrderEmailSendRequest DeserializeQueueMessage(string queueMessage)
        {
            return JsonConvert.DeserializeObject<PurchaseOrderEmailSendRequest>(queueMessage);
        }

        [FunctionName(nameof(LoadPurchaseOrder))]
        public PurchaseOrderInfo LoadPurchaseOrder([ActivityTrigger] Guid purchaseOrderId, ILogger log)
        {
            string sql = @$"
                        declare @TaxRate decimal(9,2) = 0, @TaxRateM decimal(9,2) = 0

                        SELECT @TaxRate = ISNULL(SUM(Rate),0) FROM [dbo].[PurchaseOrderAdditionalCharges] AC 
                        WHERE AC.PurchaseOrderId = @PurchaseOrderId and AC.[Type]=1 and AC.IsActive=1;;


                        ;WITH cte_purchaseorder AS (
                            SELECT P.[Id] [PurchaseOrderId], P.CompanyId, P.SupplierId, 
                                   O.Name [Owner_Name], 
                                   S.Name [Supplier_Name]
                            FROM [dbo].[PurchaseOrders] P
                                LEFT JOIN [dbo].[Organisations] S ON S.Id = P.SupplierId
                                LEFT JOIN [dbo].[Organisations] O ON O.Id = P.CompanyId
                            WHERE P.[Active] = 1 AND P.Id = @PurchaseOrderId  
                        ),
                        cte_address AS (
                            SELECT TOP 1 PO.[PurchaseOrderId], A.[Name] [Buyer_Name], 
                                A.AddressLine1 [Buyer_Address1], A.AddressLine2 [Buyer_Address2], A.AddressLine3 [Buyer_Address3],
                                A.AddressLine4 [Buyer_Address4], A.City [Buyer_City], A.County [Buyer_County], A.PostalCode [Buyer_PostalCode], C.[Name] [Buyer_Country] 
                            FROM cte_purchaseorder PO
                                LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = PO.CompanyId 
                                LEFT JOIN [dbo].[Countries] C ON C.Code2 = A.CountryCode
                            WHERE A.IsInvoiceAddress = 1

                        )
                        SELECT  P.Owner_Name, P.Supplier_Name, A.*,
                                PO.DateOfIssue, PO.GoodsReadyDate, PO.Reference, PO.PaymentTerms, PO.AdditionalSupplierInformation,
                                PO.Total_CurrencyId, PO.Total_NetAmount, PO.Total_TaxAmount, PO.Total_TotalAmount, PO.PurchaseOrderNumber, PO.AdditionalSupplierInformation, PO.CurrencyId [Currency]
                        FROM cte_purchaseorder [P]
                            INNER JOIN [dbo].[PurchaseOrders] PO ON PO.Id = P.PurchaseOrderId
                            LEFT JOIN cte_address A ON A.PurchaseOrderId = P.PurchaseOrderId


                        SET @TaxRateM = @TaxRate / 100.00

                        ;WITH cte_line_values AS (
                            SELECT POI.PurchaseOrderId, POI.Id [ItemId], 'I' [Type], 
                            ISNULL(POI.PurchaseOrderItemText, ISNULL(P.Description,PV.Description)) [Description],
                            ISNULL(POI.SKU, ISNULL(P.SKU,PV.SKU)) [SKU],
                            [OrderQuantity] [OrderQuantity], 
                            ISNULL([OrderPriceUnit], 0) [OrderPriceUnit], 
                            (ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0)) [NetAmount], 
                            ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0))*@TaxRateM) [Vat],
                            ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0)) + ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0))*@TaxRateM)) [TotalAmount], POI.CreationDateInternal, 0 [Seq]
                            FROM [dbo].[PurchaseOrderItems] POI
                                LEFT JOIN [dbo].[Products] P ON P.Id = POI.ProductId
                                LEFT JOIN [dbo].[ProductVariants] PV ON PV.Id = POI.ProductId
                            WHERE POI.PurchaseOrderId = @PurchaseOrderId and POI.[Active] = 1
                            UNION
                            SELECT POC.PurchaseOrderId, POC.Id [ItemId], 'C' [Type], POC.[Description], NULL [SKU],
                            1 [OrderQuantity], 
                            ISNULL(Rate,0) [OrderPriceUnit], 
                            1 * ISNULL(Rate,0) [NetAmount], 
                            (1 * ISNULL(Rate,0))*(0) [Vat], 
                            ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(0))) [TotalAmount], POC.CreationDateInternal, 1 [Seq] 
                        -- (1 * ISNULL(Rate,0))*(@TaxRateM) [Vat],
                        -- ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(@TaxRateM))) [TotalAmount]
                            FROM [dbo].[PurchaseOrderAdditionalCharges] POC 
                            WHERE POC.PurchaseOrderId = @PurchaseOrderId and POC.[IsActive] = 1 and POC.[Type] = 2
                            UNION
                            SELECT POC.PurchaseOrderId, POC.Id [ItemId], 'T' [Type], POC.[Description], NULL [SKU],
                            1 [OrderQuantity], 
                            ISNULL(Rate,0) [OrderPriceUnit], 
                            1 * ISNULL(Rate,0) [NetAmount], 
                            (1 * ISNULL(Rate,0))*(0) [Vat], 
                            ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(0))) [TotalAmount], POC.CreationDateInternal, 1 [Seq] 
                        -- (1 * ISNULL(Rate,0))*(@TaxRateM) [Vat],
                        -- ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(@TaxRateM))) [TotalAmount]
                            FROM [dbo].[PurchaseOrderAdditionalCharges] POC 
                            WHERE POC.PurchaseOrderId = @PurchaseOrderId and POC.[IsActive] = 1 and POC.[Type] = 1
                        )
                        SELECT * FROM cte_line_values
                        ORDER BY Seq, CreationDateInternal";
            PurchaseOrderInfo po;

            using (IDbConnection conn = new SqlConnection(TradefactDBConnectionString))
            {
                conn.Open();

                using (var multi = conn.QueryMultiple(sql, new
                {
                    PurchaseOrderId = purchaseOrderId
                }))
                {
                    po = multi.Read<PurchaseOrderInfo>().FirstOrDefault();
                    List<PurchaseOrderItemInfo> po_items = multi.Read<PurchaseOrderItemInfo>().ToList();

                    if (po != null)
                    {
                        po.Items = po_items.Where(q => q.Type == "I").ToList();
                        po.Charges = po_items.Where(q => q.Type == "C").ToList();
                        po.Taxes = po_items.Where(q => q.Type == "T").ToList();
                    }
                }
            }
            return po;
        }

        [FunctionName(nameof(GeneratePurchaseOrderPDF))]
        public async Task<byte[]> GeneratePurchaseOrderPDF([ActivityTrigger] PurchaseOrderInfo po, ILogger log)
        {
            var payload = new
            {
                template = new
                {
                    name = "purchaseorder-main"
                },
                data = po
            };

            var serialized_payload = JsonConvert.SerializeObject(payload);
            var data = new StringContent(serialized_payload, Encoding.UTF8, "application/json");

            const string filename = "TradefactPO.pdf";
            HttpResponseMessage response = await httpClient.PostAsync("https://tradefactreportfn.azurewebsites.net/api/HttpTrigger1", data);
            Stream outputStream = await response.Content.ReadAsStreamAsync();

            byte[] bytes = new byte[outputStream.Length];
            outputStream.Read(bytes, 0, (int)outputStream.Length);

            return bytes;
        }


        [FunctionName(nameof(SendPOEmails))]
        public async Task SendPOEmails(
            [DurableClient] IDurableOrchestrationClient starter,
            [ServiceBusTrigger("pocreatedqueue-test", Connection = "ServiceBusConnection")] Microsoft.Azure.ServiceBus.Message message,
            MessageReceiver messageReceiver,
            string lockToken,
            ILogger log)
        {
            string inputMessage = Encoding.UTF8.GetString(message.Body);
            log.LogInformation($"message - " + inputMessage);
            if (string.IsNullOrWhiteSpace(inputMessage)) await messageReceiver.DeadLetterAsync(lockToken, "Message content is empty.", "Message content is empty.");

            string instanceId = await starter.StartNewAsync(nameof(SendPOEmailOrchestrator), inputMessage);
            log.LogInformation($"Orchestration Started with ID: {instanceId}");


            var orchestrationStatus = await starter.GetStatusAsync(instanceId);
            var status = orchestrationStatus.RuntimeStatus.ToString().ToUpper();
            log.LogInformation($"Waiting to complete Orchestration function [Status:{status}][ID:{instanceId}]");

            while (status == "PENDING" || status == "RUNNING")
            {
                await Task.Delay(1000);
                orchestrationStatus = await starter.GetStatusAsync(instanceId);
                status = orchestrationStatus.RuntimeStatus.ToString().ToUpper();
            }

            log.LogInformation($"PO-Email-Send-Function completed [Instance ID:{instanceId}]");

            if (!(bool)orchestrationStatus.Output)
            {
                await messageReceiver.AbandonAsync(lockToken);
            }
        }

        [FunctionName(nameof(SendPOEmailsDLQ))]
        public static async void SendPOEmailsDLQ([ServiceBusTrigger("pocreatedqueue-test/$DeadLetterQueue", Connection = "ServiceBusConnection")] Microsoft.Azure.ServiceBus.Message message,
            MessageReceiver messageReceiver,
            string lockToken,
            ILogger log)
        {
            string inputMessage = Encoding.UTF8.GetString(message.Body);
            log.LogInformation($"message - " + inputMessage);
        }


        [FunctionName(nameof(ResendPOEmail))]
        [return: ServiceBus("pocreatedqueue-test", Connection = "ServiceBusConnection")]
        public static async Task<string> ResendPOEmail(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
    HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            return requestBody;
        }
    }
}
