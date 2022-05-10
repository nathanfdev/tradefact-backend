using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.External.API.Model
{
    public class PurchaseOrder
    {
        [JsonProperty("id")]
        public string PurchaseOrderID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Reference { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? GoodsReadyDate { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string PlaceOfIssue { get; set; }
        public string Status { get; set; }

        public double CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public double SubTotal { get; set; }
        public double TotalTax { get; set; }
        public double Total { get; set; }
        public string PaymentTerms { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public List<string> Tags { get; set; }
        public List<LineItem> LineItems { get; set; }
    }

    public class LineItem
    {
        public string SKU { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string TaxType { get; set; }
        public double TaxAmount { get; set; }
        public double LineAmount { get; set; }
        public string LineItemID { get; set; }
        public List<string> Tags { get; set; }
    }
}
