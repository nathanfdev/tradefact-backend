using Core.Attributes;
using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderAdditionalChargeItemResource
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("PurchaseAdditionalChargeItemId")]
        public Guid Id { get; set; }
        public ChargeItemTypeEnum Type { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }
        
        [JsonProperty("Rate")]
        public decimal Rate { get; set; }

        //[JsonProperty("Total")]
        [JsonIgnore]
        public Total Total { get; set; }

        // [JsonProperty("BaseCurrencyTotal")]
        [JsonIgnore]
        public Total BaseCurrencyTotal { get; set; }
    }

}
