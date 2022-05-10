using Newtonsoft.Json;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public partial class PurchaseOrderNoteResource
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderNoteId { get; set; }

        [JsonProperty("TextObjectType")]
        public string TextObjectType { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("PlainLongText")]
        public string PlainLongText { get; set; }
    }
}
