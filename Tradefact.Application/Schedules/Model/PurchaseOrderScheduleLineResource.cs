using Core.Attributes;
using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tradefact.Application.Models;
using X.PagedList;

namespace Tradefact.Application.Schedules.Model
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderScheduleLineResourceDTO
    {
        public Guid PurchaseOrderId { get; set; }

        public Guid ScheduleLineId { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string Reference { get; set; }

        public string Name { get; set; }

        public DateTime? GoodsReady { get; set; }

        public int Lines { get; set; }

        public int Items { get; set; }

        public string CurrencyId { get; set; }

        public decimal LineValue { get; set; }

        public string CountryofLoadingCode { get; set; }

        public string Country { get; set; }

        public Guid PlaceOfLoadingId { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PlaceOfLoading_Address1 { get; set; }
        public string PlaceOfLoading_Address2 { get; set; }
        public string PlaceOfLoading_Address3 { get; set; }
        public string PlaceOfLoading_Address4 { get; set; }

        public string PlaceOfLoading_City { get; set; }

        public string Supplier { get; set; }

        public bool IsLocked { get; set; }
    }


    public partial class PurchaseOrderScheduleLineItemResourceDTO
    {
        public Guid? PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderItemId { get; set; }

        public Guid? PurchaseOrderItemScheduleLineId { get; set; }

        public DateTimeOffset RequestedDeliveryDate { get; set; }

        public long ScheduleLineOrderQuantity { get; set; }

        public decimal ScheduleLineCommittedQuantity { get; set; }

        public long ScheduleLineOrderWeight { get; set; }

        public string PortOfLoadingCode { get; set; }

        public string CountryofLoadingCode { get; set; }

        public string CreatedByUser { get; set; }

        public DateTimeOffset CreationDateInternal { get; set; }

        public string LastChangeUser { get; set; }

        public DateTimeOffset LastModifiedOnInternal { get; set; }

        public Guid ProductId { get; set; }

        public decimal Product_DimensionsHeight { get; set; }

        public decimal Product_DimensionsLength { get; set; }

        public string Product_DimensionsScale { get; set; }

        public decimal Product_DimensionsWidth { get; set; }

        public string Product_GoodsType { get; set; }

        public string Product_HsCode { get; set; }

        public string Product_Name { get; set; }

        public string Product_Description { get; set; }

        public string Product_Nickname { get; set; }

        public string Product_Packing { get; set; }

        public string Product_Sku { get; set; }

        public Guid Product_CompanyId { get; set; }

        public string Product_HazardClass { get; set; }

        public string Product_HazardNotes { get; set; }

        public bool Product_MagneticFieldContained { get; set; }

        public int Product_UnitsPerPackage { get; set; }

        public int Product_HazardousContents { get; set; }

        public bool Product_Rotatable { get; set; }

        public bool Product_Stackable { get; set; }
        public bool ProductDescriptionOverride { get; set; }

        public long Product_DimensionsWeight { get; set; }

        public string Product_DimensionsWeightMeasurement { get; set; }

        public string Product_LithiumBatteryPacking { get; set; }

        public string CurrencyId { get; set; }

        public decimal LineValue { get; set; }

    }

    public partial class PurchaseOrderScheduleLineItemResource
    {
        [JsonProperty("RequestedDeliveryDate")]
        public DateTimeOffset RequestedDeliveryDate { get; set; }

        [JsonProperty("ScheduleLineOrderQuantity")]
        public long ScheduleLineOrderQuantity { get; set; }

        [JsonProperty("ScheduleLineCommittedQuantity")]
        public decimal ScheduleLineCommittedQuantity { get; set; }

        [JsonProperty("ScheduleLineOrderWeight")]
        public long ScheduleLineOrderWeight { get; set; }

        [JsonProperty("PortOfLoadingCode")]
        public string PortOfLoadingCode { get; set; }

        [JsonProperty("CountryofLoadingCode")]
        public string CountryofLoadingCode { get; set; }

        [JsonProperty("CreatedByUser")]
        public string CreatedByUser { get; set; }

        [JsonProperty("CreationDateInternal")]
        public DateTimeOffset CreationDateInternal { get; set; }

        [JsonProperty("LastChangeUser")]
        public string LastChangeUser { get; set; }

        [JsonProperty("LastModifiedOnInternal")]
        public DateTimeOffset LastModifiedOnInternal { get; set; }

        [JsonProperty("ProductId")]
        public ProductResource Product { get; set; }

        [JsonProperty("Value")]
        public Total Value { get; set; }

        public Guid? PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderItemId { get; set; }

        public Guid? PurchaseOrderItemScheduleLineId { get; set; }

        public Guid? PlaceOfLoadingId { get; set; }
    }


    public class PurchaseOrderScheduleLineResource
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("ScheduleLineId")]
        public Guid ScheduleLineId { get; set; }

        [JsonProperty("PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("GoodsReady")]
        public DateTime? GoodsReady { get; set; }

        [JsonProperty("Lines")]
        public int Lines { get; set; }

        [JsonProperty("ItemQty")]
        public int Items { get; set; }

        [JsonIgnore]
        public Guid PlaceOfLoadingId { get; set; }

        [JsonProperty("PlaceOfLoading")]
        public Address PlaceOfLoading { get; set; }

        [JsonProperty("Supplier")]
        public string Supplier { get; set; }

        [JsonProperty("IsLocked")]
        public bool IsLocked { get; set; }

        [JsonProperty("Value")]
        public Total Value { get; set; }

        [JsonProperty("Items")]
        public IPagedList<PurchaseOrderScheduleLineItemResource> Schedule { get; set; }
    }
}
