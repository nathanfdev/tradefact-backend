using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class PurchaseOrderItem : BaseEntity<PurchaseOrderItem>
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("ProductId")]
        public Guid ProductId { get; set; }

        //[JsonProperty("ProductVariantId")]
        //public Guid? ProductVariantId { get; set; }

        //[JsonProperty("IsProductVariant")]
        //public bool IsProductVariant { get; set; } = false;

        [JsonProperty("SKU")]
        public string SKU { get; set; }

        [JsonProperty("PurchaseOrderItemText")]
        public string PurchaseOrderItemText { get; set; }

        [JsonProperty("OrderQuantity")]
        public decimal OrderQuantity { get; set; }

        [JsonProperty("PurchaseOrderQuantityUnit")]
        public string OrderQuantityUnit { get; set; }

        [JsonProperty("OrderPriceUnit")]
        public decimal OrderPriceUnit { get; set; }

        [JsonProperty("NetPriceAmount")]

        public decimal NetPriceAmount { get; set; }

        [JsonProperty("NetPriceQuantity")]

        public int NetPriceQuantity { get; set; }

        [JsonProperty("TaxCode")]
        public string TaxCode { get; set; }

        [JsonProperty("TaxDeterminationDate")]
        public string TaxDeterminationDate { get; set; }

        [JsonProperty("TaxCountry")]
        public string TaxCountry { get; set; }

        [JsonProperty("TaxJurisdiction")]
        public string TaxJurisdiction { get; set; }

        [JsonProperty("IsCompletelyDelivered")]
        public bool IsDeliveryComplete { get; set; }

        [JsonProperty("IsFinallyInvoiced")]
        public bool IsFinallyInvoiced { get; set; }

        [JsonProperty("PurchaseOrderItemCategory")]
        public string PurchaseOrderItemCategory { get; set; }

        [JsonProperty("AccountAssignmentCategory")]
        public string AccountAssignmentCategory { get; set; }

        [JsonProperty("PurchaseContract")]
        public string PurchaseContract { get; set; }

        [JsonProperty("ItemNetWeight")]

        public decimal ItemNetWeight { get; set; }

        [JsonProperty("ItemWeightUnit")]
        public string ItemWeightUnit { get; set; }


        [JsonProperty("ItemVolume")]

        public decimal ItemVolume { get; set; }

        [JsonProperty("ItemVolumeUnit")]
        public string ItemVolumeUnit { get; set; }

        [JsonProperty("SupplierReference")]
        public string SupplierReference { get; set; }

        //[JsonIgnore]
        //public Product Product { get; set; }

        //[JsonIgnore]
        //public ProductVariant ProductVariant { get; set; }

        [JsonProperty("PurchaseOrderItemNotes")]
        public ICollection<PurchaseOrderItemNote> PurchaseOrderItemNotes { get; set; }

        //[JsonProperty("PurchaseOrderPricingElement")]
        //public PurchaseOrderPricingElement PurchaseOrderPricingElement { get; set; }

        [JsonProperty("ScheduleLines")]
        public List<PurchaseOrderItemScheduleLine> ScheduleLines { get; set; }

        // Navigation
        public PurchaseOrder PurchaseOrder { get; set; }

        public bool IsBulkUpload { get; set; }

    }

}
