using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradefact.Application.Models
{
    public class InboxPurchaseOrderResource
    {
        public Guid Id { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Reference { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public string SupplierName { get; set; }
        public Guid? SupplierId { get; set; }
        public bool IsDuplicatePONumber { get; set; }
        public int NoOfLines { get; set; }
        public int SKUNotFoundCount { get; set; }
        public int IncompleteProductCount { get; set; }
        public Guid? GenericProductId { get; set; }
        public string Source { get; set; }

        [JsonIgnore]
        public virtual string Tags { get; set; }

        [JsonProperty("tags")]
        public List<string> TagsList => (this.Tags == null) ? new List<string>() : this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

    }
}
