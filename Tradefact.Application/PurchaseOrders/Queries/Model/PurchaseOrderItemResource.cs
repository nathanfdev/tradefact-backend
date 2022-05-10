using Core.Attributes;
using Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tradefact.Application.Models;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderItemResource
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("PurchaseOrderItemId")]
        public Guid PurchaseOrderItemId { get; set; }

        [JsonProperty("Product")]
        public ProductResource Product { get; set; }

        [JsonIgnore]
        public Guid? ProductVariantId { get; set; }

        [JsonProperty("IsProductVariant")]
        public bool IsProductVariant { get; set; } = false;

        [JsonProperty("SKU")]
        public string SKU { get; set; }

        [JsonProperty("PurchaseOrderItemText")]
        public string PurchaseOrderItemText { get; set; }

        [JsonProperty("OrderQuantity")]
        public decimal OrderQuantity { get; set; }

        [JsonProperty("ScheduleLineCommittedQuantity")]
        public decimal ScheduleLineCommittedQuantity { get; set; }

        [JsonProperty("RemainingQuantity")]
        public decimal RemainingQuantity { get; set; }

        [JsonProperty("PurchaseOrderQuantityUnit")]
        public string OrderQuantityUnit { get; set; }

        [JsonProperty("OrderPriceUnit")]
        public string OrderPriceUnit { get; set; }

        [JsonProperty("SupplierReference")]
        public string SupplierReference { get; set; }

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

        public bool ProductDescriptionOverride { get; set; }


        public int NoSchedules { get; set; }
        public int NoShipments { get; set; }

        public int ShipmentsBooked { get; set; }
        public int ShipmentsInTransit { get; set; }
        public int ShipmentsDelivered { get; set; }
        public int ShipmentsQuotationRequested { get; set; }

        public int AirShipments { get; set; }
        public int SeaShipments { get; set; }
        public int RoadShipments { get; set; }
        public int RailShipments { get; set; }

        public OrderShipmentStatusEnum OrderShipmentStatus { get; set; }

        public List<PurchaseOrderScheduleInformationResource> Schedules { get; set; }
        public List<PurchaseOrderShipmentInformationResource> Shipments { get; set; }
    }

}
