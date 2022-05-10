using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public partial class PurchaseOrderItemNote : BaseEntity<PurchaseOrderItemNote>
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("PurchaseOrderItemId")]
        public Guid PurchaseOrderItemId { get; set; }

        [JsonProperty("TextObjectType")]
        public string TextObjectType { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("PlainLongText")]
        public string PlainLongText { get; set; }

        public PurchaseOrderItem PurchaseOrderItem { get; set; }
    }

}
