using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public partial class PurchaseOrderItemScheduleLine : BaseEntity<PurchaseOrderItemScheduleLine>
    {
        [JsonProperty("purchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("purchaseOrderItemId")]
        public Guid PurchaseOrderItemId { get; set; }


        [JsonProperty("description")]
        public string Description { get; set; }

        
        [JsonProperty("requestedGoodsReadyDate")]
        public DateTime? RequestedGoodsReadyDate { get; set; }

        [JsonProperty("confirmedGoodsReadyDate")]
        public DateTime? ConfirmedGoodsReadyDate { get; set; }


        [JsonProperty("requestedDeliveryDate")]
        public DateTime? RequestedDeliveryDate { get; set; }

        [JsonProperty("confirmedDeliveryDate")]
        public DateTime? ConfirmedDeliveryDate { get; set; }

        
        [JsonProperty("scheduleLineOrderQuantity")]
        public int ScheduleLineOrderQuantity { get; set; }

        [JsonProperty("ScheduleLineCommittedQuantity")]
        public decimal ScheduleLineCommittedQuantity { get; set; }


        [JsonProperty("OrderQuantityUnit")]
        public string OrderQuantityUnit { get; set; }

        [JsonProperty("scheduleLineOrderWeight")]
        public decimal ScheduleLineOrderWeight { get; set; }

        public Guid? PlaceOfLoadingId { get; set; }
        public Address PlaceOfLoading { get; set; }

        [JsonProperty("countryCode")]
        public string CountryofLoadingCode { get; set; }

        [JsonProperty("countryOfLoading")]
        public Country CountryOfLoading { get; set; }

        public PurchaseOrderItem PurchaseOrderItem { get; set; }
    }

}
