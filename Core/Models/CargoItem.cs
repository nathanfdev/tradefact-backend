using Core.Attributes;
using Newtonsoft.Json;
using System;

namespace Core.Models
{
    [TypescriptAutoGeneration]
    public class CargoItem
    {
        [JsonIgnore]
        public Guid FreightMovementId { get; set; }
        [JsonIgnore]
        public Guid FreightMovementItemId { get; set; }
        [JsonIgnore]
        public Guid ItemId { get; set; }


        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public bool IsProductVariant { get; set; } = false;
        public string HsCode { get; set; }
        public string SKU { get; set; }
        public string ItemDescription { get; set; }

        public decimal Qty { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public string UOL { get; set; }
        public int CartonQty { get; set; }
        public bool ProductDescriptionOverride { get; set; }

        public decimal Weight { get; set; }
        public string UOW { get; set; }

        [JsonIgnore]
        public FreightMovementItem FreightMovementItem { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }

        [JsonIgnore]
        public ProductVariant ProductVariant { get; set; }

        public Guid? PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderItemId { get; set; }
        public Guid? PurchaseOrderItemScheduleLineId { get; set; }

        public Guid? PlaceOfLoadingId { get; set; }
        [JsonIgnore]
        public Address PlaceOfLoading { get; set; }
    }
}
