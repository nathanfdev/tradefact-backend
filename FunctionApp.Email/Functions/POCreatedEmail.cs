using System;
using Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Core.Enums;
using FunctionApp.Email.Common;
using Core.Models.Email;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace FunctionApp.Email
{
    public static class POCreatedEmail
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("POCreatdEmail")]
        public static async void Run([ServiceBusTrigger("pocreatedqueue", Connection = "connectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

			var message = DeserializeQueueMessage(myQueueItem);
            string template = "pocreatedunregisteredsupplier";

            // ------------------------------------------------------------------------------------------------------
            var str = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);
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
                        cte_buyer_address AS (
                            SELECT TOP 1 PO.[PurchaseOrderId], A.[Name] [Buyer_Name], 
                                A.AddressLine1 [Buyer_Address1], A.AddressLine2 [Buyer_Address2], A.AddressLine3 [Buyer_Address3],
                                A.AddressLine4 [Buyer_Address4], A.City [Buyer_City], A.County [Buyer_County], A.PostalCode [Buyer_PostalCode], C.[Name] [Buyer_Country] 
                            FROM cte_purchaseorder PO
                                LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = PO.CompanyId 
                                LEFT JOIN [dbo].[Countries] C ON C.Code2 = A.CountryCode
                            WHERE A.IsInvoiceAddress = 1
                        ),
                        cte_supplier_address AS (
                            SELECT TOP 1 PO.[PurchaseOrderId], 
                                A.AddressLine1 [Supplier_Address1], A.AddressLine2 [Supplier_Address2], A.AddressLine3 [Supplier_Address3],
                                A.AddressLine4 [Supplier_Address4], A.City [Supplier_City], A.County [Supplier_County], A.PostalCode [Supplier_PostalCode], C.[Name] [Supplier_Country] 
                            FROM cte_purchaseorder PO
                                LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = PO.SupplierId 
                                LEFT JOIN [dbo].[Countries] C ON C.Code2 = A.CountryCode
                            ORDER BY A.IsInvoiceAddress DESC
                        )
                        SELECT  P.Owner_Name, P.Supplier_Name, A.*, S.*,
                                PO.DateOfIssue, PO.GoodsReadyDate, PO.Reference, PO.PaymentTerms, PO.AdditionalSupplierInformation,
                                PO.Total_CurrencyId, PO.Total_NetAmount, PO.Total_TaxAmount, PO.Total_TotalAmount, PO.PurchaseOrderNumber, PO.AdditionalSupplierInformation, PO.CurrencyId [Currency]
                        FROM cte_purchaseorder [P]
                            INNER JOIN [dbo].[PurchaseOrders] PO ON PO.Id = P.PurchaseOrderId
                            LEFT JOIN cte_buyer_address A ON A.PurchaseOrderId = P.PurchaseOrderId
                            LEFT JOIN cte_supplier_address S ON S.PurchaseOrderId = P.PurchaseOrderId

                        SET @TaxRateM = @TaxRate / 100.00

                        ;WITH cte_line_values AS (
                            SELECT POI.PurchaseOrderId, POI.Id [ItemId], 'I' [Type], 
                            ISNULL(POI.PurchaseOrderItemText, ISNULL(P.Name,PV.Name)) [Description],
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
             
            using (IDbConnection conn = new SqlConnection(str))
            {
                conn.Open();

                using (var multi = conn.QueryMultiple(sql, new
                {
                    PurchaseOrderId = message.Body.PurchaseOrderId
                }))
                {
                    po = multi.Read<PurchaseOrderInfo>().FirstOrDefault();
                    List<PurchaseOrderItemInfo> po_items = multi.Read<PurchaseOrderItemInfo>().ToList();

                    if(po != null) {
                        po.Items = po_items.Where(q => q.Type == "I").ToList();
                        po.Charges = po_items.Where(q => q.Type == "C").ToList();
                        po.Taxes = po_items.Where(q => q.Type == "T").ToList();
                    }
                }
            }

            if(po != null) { 
                //-----------------------------------------------------------------------------------------------------------
                //Create the PDF from message.PurchaseOrder
                var templatename = new
                {
                    name = "purchaseorder-main"
                };  

                var values = new 
                {
                    template = templatename,
                    data = po
                }; 

                var serialized = JsonConvert.SerializeObject(values);
                var data = new StringContent(serialized, Encoding.UTF8, "application/json");
            
                const string filename = "TradefactPO.pdf";
                HttpResponseMessage response = await httpClient.PostAsync("https://tradefactreportfn.azurewebsites.net/api/HttpTrigger1", data);
                Stream outputStream = await response.Content.ReadAsStreamAsync();
                DateTime.Now.ToString();

                byte[] bytes = new byte[outputStream.Length];
                outputStream.Read(bytes, 0, (int)outputStream.Length);
                // -------------------------------------------------------------------------------------------------------

                string organisationName = po.Owner_Name;
                foreach (var item in message.Body.Recipients)
                {
                    var variables = new
                    {
                        first_name = item.FullName,
                        organisation_name = organisationName,
                        link = item.RegistrationLink
                    };

                    BaseEmail email = new BaseEmail()
                    {
                        Email = item.Email
                        //Email = "mike.bucher@tradefact.com"
                    };

                    EmailSender.SendMessageWithAttachments(email, JsonConvert.SerializeObject(variables), template, "Tradefact: A new purchase order was created", bytes, filename);
                }
            }

        }

		private static ServiceBusInviteEmail<PurchaseOrderEmailSendRequest> DeserializeQueueMessage(string queueMessage)
        {
			return (ServiceBusInviteEmail<PurchaseOrderEmailSendRequest>)JsonConvert.DeserializeObject(queueMessage, typeof (ServiceBusInviteEmail<PurchaseOrderEmailSendRequest>));
        }
        
    }
}
