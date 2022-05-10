using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos.PurchaseOrder
{
    public class PurchaseOrderFlatfileImportRequest
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid? PurchaseOrderId { get; set; }

        [JsonProperty("ProductId")]
        public Guid? ProductId { get; set; }


        // ---------------------------------------
        // External Via CSV

        [JsonProperty("BatchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("SupplierId")]
        public Guid? SupplierId { get; set; }

        [JsonProperty("PurchaseOrderNo")]
        public string PurchaseOrderNo{ get; set; }

        [JsonProperty("CurrencyCode")]
        public string CurrencyCode { get; set; }
    }
}
