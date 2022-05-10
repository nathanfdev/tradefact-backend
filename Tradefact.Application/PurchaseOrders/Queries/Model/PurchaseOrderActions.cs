using Core.Attributes;
using Newtonsoft.Json;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderActions
    {
        [JsonIgnore]
        public bool OrderActive { get; set; }
        [JsonIgnore]
        public int OrderStatus { get; set; }

        public bool IsOwned { get; set; }

        [JsonProperty(Order = 1)]
        public bool Edit { get; set; } = false;
        [JsonProperty(Order = 1)]
        public bool Submit { get; set; } = false;

        [JsonProperty(Order = 2)]
        public bool Accept { get; set; } = false;

        [JsonProperty(Order = 2)]
        public bool Reject { get; set; } = false;

        [JsonProperty(Order = 3)]
        public bool Cancel { get; set; } = false;

        [JsonProperty(Order = 4)]
        public bool AddGoodsReadyDate { get; set; } = false;

        [JsonProperty(Order = 4)]
        public bool EditGoodsReadyDate { get; set; } = false;

        [JsonProperty(Order = 5)]
        public bool SetProductionStatus { get; set; } = false;  // Only available when estimated date departure reached

        [JsonProperty(Order = 7)]
        public bool SetShippingStatus { get; set; } = false;

        [JsonProperty(Order = 8)]
        public bool SetPreShippingStatus { get; set; } = false;

        [JsonProperty(Order = 9)]
        public bool Complete { get; set; } = false;

        [JsonProperty(Order = 10)]
        public bool RequestShippingQuotation { get; set; } = false;

        [JsonIgnore]
        public bool Reset { get; set; } = false;

        [JsonProperty(Order = 12)]
        public bool Resend { get; set; } = false;

        [JsonProperty("delete")]
        public bool CanDelete => ((this.Reset || this.OrderStatus == 0) && this.IsOwned && this.OrderActive);

        [JsonProperty("reset")]
        public bool CanReset => (this.Reset && this.IsOwned && this.OrderActive);

    }

}
