using Newtonsoft.Json;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public partial class PurchaseOrderItemNoteResource : PurchaseOrderNoteResource
    {
        [JsonProperty("PurchaseOrderItemId")]
        public Guid PurchaseOrderItemId { get; set; }

        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderNoteId { get; set; }
    }

}
