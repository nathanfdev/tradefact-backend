using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public partial class PurchaseOrderFlatfileImport
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid? PurchaseOrderId { get; set; }

        [JsonProperty("UserId")]
        public string UserId { get; set; }

        [JsonProperty("BatchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("ProductId")]
        public Guid? ProductId { get; set; }

        [JsonProperty("CorrelationId")]
        public Guid CorrelationId { get; set; }



        [JsonProperty("SupplierId")]
        public Guid? SupplierId { get; set; }

        [JsonProperty("PurchaseOrderNo")]
        public string PurchaseOrderNo { get; set; }

        [JsonProperty("CurrencyCode")]
        public string CurrencyCode { get; set; }
        [JsonProperty("OrganisationId")]
        public Guid OrganisationId { get; set; }
    }
}