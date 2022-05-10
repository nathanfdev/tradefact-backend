using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public partial class PurchaseOrderNote : BaseEntity<PurchaseOrderNote>
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("TextObjectType")]
        public string TextObjectType { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("PlainLongText")]
        public string PlainLongText { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
    }
}