using Core.Attributes;
using Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Models
{
    public partial class PurchaseOrder : BaseEntity<PurchaseOrder>
    {
        [JsonIgnore]
        public Guid CompanyId { get; set; }

        /// <summary>Supplier Id</summary>
        [JsonProperty("SupplierId")]
        public Guid? SupplierId { get; set; }

        [JsonProperty("PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("PurchaseOrderDate")]
        public DateTime? PurchaseOrderDate { get; set; }

        [JsonProperty("GoodsReadyDate")]
        public DateTime? GoodsReadyDate { get; set; }

        [JsonProperty("TargetDeliveryDate")]
        public DateTime? TargetDeliveryDate { get; set; }

        [JsonProperty("IncotermsVersion")]
        public string IncotermsVersion { get; set; }

        // Status & Stage
        public PurchaseOrderStatus Status { get; set; }
        public PurchaseOrderStage Stage { get; set; }

        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum LoadType { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }

        // Only for EXW
        public Guid? PlaceOfLoadingId { get; set; }
        public string PortOfLoadingId { get; set; }
        public string PortOfDischargeId { get; set; }
        public Guid? PlaceOfDispatchId { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("PurchasingGroup")]
        public string PurchasingGroup { get; set; }

        [JsonProperty("CorrespncExternalReference")]
        public string CorrespncExternalReference { get; set; }

        [JsonProperty("CorrespncInternalReference")]
        public string CorrespncInternalReference { get; set; }


        [JsonIgnore]
        public virtual string Tags { get; set; }

        [JsonIgnore]
        public string Containers { get; set; }

        [JsonProperty("tags")]
        public List<string> TagsList => (this.Tags == null) ? new List<string>() : this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        [JsonProperty("containers")]
        public List<string> ContainerList => (this.Containers == null) ? new List<string>() : this.Containers.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        public Location PortOfLoading { get; set; }
        public Location PortOfDischarge { get; set; }
        public Address PlaceOfLoading { get; set; }
        public Address PlaceOfDispatch { get; set; }

        public string CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal InverseExchangeRate { get; set; }

        public int ValidForDays { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string PaymentTerms { get; set; }

        public bool? CustomsBrokerageRequired { get; set; } = null;
        public int? NumberOfItems { get; set; } = 1;
        public string HSCodes { get; set; }

        // Insurance
        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public string Notes { get; set; }
        public string RejectedReason { get; set; }
        public bool OrderRejected { get; set; }
        public string Signature { get; set; }
        public string AdditionalInformation { get; set; }

        public decimal TaxRate { get; set; }
        public string TaxRateDescription { get; set; }

        public Total ItemsTotal { get; set; }
        public Total ChargesTotal { get; set; }
        public Total Total { get; set; }
        public Total BaseCurrency { get; set; }

        // Stages
        [JsonProperty("Submitted")]
        public bool Submitted { get; set; }
        [JsonProperty("SubmittedDate")]
        public DateTime? SubmittedDate { get; set; }

        [JsonProperty("Accepted")]
        public bool Accepted { get; set; }
        [JsonProperty("AcceptedDate")]
        public DateTime? AcceptedDate { get; set; }

        [JsonProperty("InProduction")]
        public bool InProduction { get; set; }
        [JsonProperty("InProductionDate")]
        public DateTime? InProductionDate { get; set; }


        [JsonProperty("PreShipment")]
        public bool PreShipment { get; set; }
        [JsonProperty("PreShipmentDate")]
        public DateTime? PreShipmentDate { get; set; }

        [JsonProperty("Shipped")]
        public bool Shipping { get; set; }
        [JsonProperty("ShippedDate")]
        public DateTime? ShippedDate { get; set; }
        [JsonProperty("Rejected")]
        public bool Rejected { get; set; }
        [JsonProperty("RejectedDate")]
        public DateTime? RejectedDate { get; set; }

        [JsonProperty("Cancelled")]
        public bool Cancelled { get; set; }
        [JsonProperty("CancelledDate")]
        public DateTime? CancelledDate { get; set; }

        [JsonProperty("Completed")]
        public bool Completed { get; set; }
        [JsonProperty("CompletedDate")]
        public DateTime? CompletedDate { get; set; }

        [JsonProperty("Deleted")]
        public bool Deleted { get; set; }
        [JsonProperty("DeletionDate")]
        public DateTime? DeletionDate { get; set; }

        [JsonProperty("PurchaseOrderItems")]
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        [JsonProperty("PurchaseOrderNotes")]
        public ICollection<PurchaseOrderNote> PurchaseOrderNotes { get; set; } = new List<PurchaseOrderNote>();

        [JsonProperty("PurchaseOrderAdditionalCharges")]
        public ICollection<PurchaseOrderChargeItem> AdditionalCharges { get; set; } = new List<PurchaseOrderChargeItem>();

        public ICollection<PurchaseOrderDocument> Documents { get; set; }
        public ICollection<PurchaseOrderEvent> Activity { get; set; }
        public ICollection<PurchaseOrderAttachedProductDocument> AttachedProductDocuments { get; set; }

        public string LogisticsNotes { get; set; }
        public string AdditionalSupplierInformation { get; set; }

        public bool IsLocked { get; set; } = false;
    }

    public enum PurchaseOrderStatus
    {
        Draft = 0,                   // Created not yet sent
        Pending = 10,                // Sent not yet accepted
        Accepted = 20,               // Accepted by Supplier
        Production = 30,             
        PreShipment = 40,
        Shipping = 50,
        Rejected = 60,               // Rejected by Supplier
        Cancelled = 70,              // Cancelled by Shipper

        Complete = 100,              // Cancelled bvy Shipper
        Archived = 110               // Archived
    }

    [Flags]
    public enum PurchaseOrderStage
    {
        Edit,
        // Waiting for Collection
        AwaitingAcceptance
    }

    public class PurchaseType
    {
        public int PurchaseTypeId { get; set; }
        public string PurchaseTypeName { get; set; }
        public string Description { get; set; }
    }

}
