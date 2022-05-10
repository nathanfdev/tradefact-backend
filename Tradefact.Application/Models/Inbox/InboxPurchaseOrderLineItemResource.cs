using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradefact.Application.Models
{
    public class InboxPurchaseOrderLineItemResource
    {
        public Guid Id { get; set;  }
        public string SKU { get; set; }
        public decimal Quantity { get; set; }
        public string CurrencyCode { get; set; }
        public decimal LineAmount { get; set; }
        public bool ExistsInCatalogue { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual string Tags { get; set; }

        [JsonProperty("tags")]
        public List<string> TagsList => (this.Tags == null) ? new List<string>() : this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        public bool ProductDataComplete { get; set; }

        public Guid ProductId { get; set; }
    }
}
