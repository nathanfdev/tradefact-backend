using Core.Attributes;
using Newtonsoft.Json;
using System;
using Tradefact.Application.Models;

namespace Tradefact.Application.Schedules.Model
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderItemScheduleLineResource
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("PurchaseOrderItemId")]
        public Guid PurchaseOrderItemId { get; set; }

        [JsonProperty("SKU")]
        public string SKU { get; set; }

        [JsonProperty("requestedGoodsReadyDate")]
        public DateTime RequestedGoodsReadyDate { get; set; }

        [JsonProperty("confirmedGoodsReadyDate")]
        public DateTime ConfirmedGoodsReadyDate { get; set; }


        [JsonProperty("requestedDeliveryDate")]
        public DateTime RequestedDeliveryDate { get; set; }

        [JsonProperty("confirmedDeliveryDate")]
        public DateTime ConfirmedDeliveryDate { get; set; }


        [JsonProperty("scheduleLineOrderQuantity")]
        public int ScheduleLineOrderQuantity { get; set; }

        [JsonProperty("ScheduleLineCommittedQuantity")]
        public decimal ScheduleLineCommittedQuantity { get; set; }


        [JsonProperty("OrderQuantityUnit")]
        public string OrderQuantityUnit { get; set; }

        [JsonProperty("scheduleLineOrderWeight")]
        public int ScheduleLineOrderWeight { get; set; }

        [JsonProperty("Product")]
        public ProductResource Product { get; set; }
    }

}
