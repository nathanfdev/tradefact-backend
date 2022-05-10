using Core.Attributes;
using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tradefact.Application.Models;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderResource
    {
        [JsonProperty("Active")]
        public bool Active { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        /// <summary>Supplier Id</summary>
        [JsonProperty("Supplier")]
        public DirectoryResource Supplier { get; set; }

        [JsonProperty("PoNumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("PurchaseOrderDate")]
        public DateTime? PurchaseOrderDate { get; set; }

        public DateTime? CreationDate { get; set; }

        [JsonProperty("GoodsReadyDate")]
        public DateTime? GoodsReadyDate { get; set; }

        [JsonProperty("TargetDeliveryDate")]
        public DateTime? TargetDeliveryDate { get; set; }

        [JsonProperty("LastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }

        [JsonProperty("IncotermsVersion")]
        public string IncotermsVersion { get; set; }

        public int? ShipmentType { get; set; }
        public int? IncoTerms { get; set; }
        // Cargo Type
        public int? LoadType { get; set; }
        public int? TransactionType { get; set; }

        public string ShipmentTypeDesc { get; set; }
        public string IncoTermsDesc { get; set; }
        // Cargo Type
        public string LoadTypeDesc { get; set; }
        public string TransactionTypeDesc { get; set; }

        // Only for EXW
        //public Guid? PlaceOfLoadingId { get; set; }
        //public string PortOfLoadingId { get; set; }
        //public string PortOfDischargeId { get; set; }
        //public Guid? PlaceOfDispatchId { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("PurchasingGroup")]
        public string PurchasingGroup { get; set; }

        [JsonProperty("CorrespncExternalReference")]
        public string CorrespncExternalReference { get; set; }

        [JsonProperty("CorrespncInternalReference")]
        public string CorrespncInternalReference { get; set; }

        [JsonProperty("PurchaseOrderItems")]
        public List<PurchaseOrderItemResource> PurchaseOrderItems { get; set; }

        [JsonProperty("PurchaseOrderAdditionalChargeItems")]
        public List<PurchaseOrderAdditionalChargeItemResource> PurchaseOrderAdditionalChargeItems { get; set; }


        [JsonProperty("PurchaseOrderNotes")]
        public List<PurchaseOrderNote> PurchaseOrderNotes { get; set; }

        [JsonProperty("Tags")]
        public List<string> Tags { get; set; }


        [JsonProperty("HSCodes")]
        public List<string> HSCodes { get; set; }

        [JsonProperty("ProductHSCodes")]
        public List<string> ProductHSCodes { get; set; }

        [JsonProperty("NoOfContainers")]
        public int NoOfContainers { get; set; }

        [JsonProperty("Containers")]
        public List<string> Containers { get; set; }


        public Port PortOfLoading { get; set; }
        public Port PortOfDischarge { get; set; }
        public AddressResource PlaceOfLoading { get; set; }
        public AddressResource PlaceOfDispatch { get; set; }

        public string Client { get; set; }
        public string Provider { get; set; }

        [JsonProperty("CreatedByName")]
        public string CreatedByName { get; set; }
        public int Status { get; set; }

        [JsonProperty("CurrencyCode")]
        public string CurrencyId { get; set; }

        [JsonProperty("ExchangeRate")]
        public decimal ExchangeRate { get; set; }

        [JsonIgnore]
        public decimal InverseExchangeRate { get; set; }

        [JsonProperty("ValidForDays")]
        public int ValidForDays { get; set; }

        [JsonProperty("DateOfIssue")]
        public DateTime? DateOfIssue { get; set; }

        [JsonProperty("PaymentTerms")]
        public string PaymentTerms { get; set; }

        [JsonProperty("CustomsBrokerageRequired")]
        public bool? CustomsBrokerageRequired { get; set; } = null;

        [JsonProperty("NumberOfItems")]
        public int? NumberOfItems { get; set; } = 1;

        // Insurance
        [JsonProperty("InsuranceRequired")]
        public bool? InsuranceRequired { get; set; } = null;

        [JsonProperty("InsuranceCurrency")]
        public string InsuranceCurrency { get; set; }

        [JsonProperty("InsuranceValue")]
        public decimal? InsuranceValue { get; set; } = 0;

        [JsonProperty("Notes")]
        public string Notes { get; set; }
    
        [JsonProperty("RejectedReason")]
        public string RejectedReason { get; set; }

        [JsonProperty("OrderRejected")]
        public bool OrderRejected { get; set; }

        [JsonProperty("Signature")]
        public string Signature { get; set; }

        [JsonIgnore]
        public decimal TaxRate { get; set; }
        [JsonIgnore]
        public string TaxRateDescription { get; set; }

        [JsonProperty("Total")]
        public Total Total { get; set; }

        [JsonProperty("ItemsTotal")]
        public Total ItemsTotal { get; set; }

        [JsonProperty("ChargesTotal")]
        public Total ChargesTotal { get; set; }


        [JsonProperty("BaseCurrency")]
        public Total BaseCurrency { get; set; }

        [JsonIgnore]
        public bool Submitted { get; set; }
        public DateTime? SubmittedDate { get; set; }

        [JsonIgnore]
        public bool Accepted { get; set; }
        public DateTime? AcceptedDate { get; set; }

        [JsonIgnore]
        public bool InProduction { get; set; }
        public DateTime? InProductionDate { get; set; }

        [JsonIgnore]
        public bool PreShipment { get; set; }
        public DateTime? PreShipmentDate { get; set; }

        [JsonIgnore]
        public bool Shipping { get; set; }
        public DateTime? ShippedDate { get; set; }
        [JsonIgnore]
        public bool Rejected { get; set; }
        public DateTime? RejectedDate { get; set; }

        [JsonIgnore]
        public bool Cancelled { get; set; }
        public DateTime? CancelledDate { get; set; }

        [JsonIgnore]
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool Deleted { get; set; }
        public DateTime? DeletionDate { get; set; }

        public string AdditionalInformation { get; set; }

        [JsonProperty(Order = 99)]
        public PurchaseOrderTimeline Timeline => this.GetPOTimeLine();

        [JsonProperty(Order = 100)]
        public PurchaseOrderActions AvailableActions => this.GetPOAvailableActions();

        [JsonProperty("LogisticsNotes")]
        public string LogisticsNotes { get; set; }

        [JsonProperty("LogisticsQuotation")]
        public PurchaseOrderShippingQuoteStatus LogisticsQuotation { get; set; }
        public string AdditionalSupplierInformation { get; set; }
        public string ViewerURL { get; set; }

        [JsonIgnore]
        public bool IsLocked { get; set; }
        public bool IsOwned { get; set; }


        private PurchaseOrderTimeline GetPOTimeLine()
        {
            PurchaseOrderTimeline t = new PurchaseOrderTimeline();

            t.Submitted = this.Submitted;
            t.SubmittedDate = this.SubmittedDate;

            t.Accepted = this.Accepted;
            t.AcceptedDate = this.AcceptedDate;

            t.Rejected = this.Rejected;
            t.RejectedDate= this.RejectedDate;

            t.InProduction = this.InProduction;
            t.InProductionDate = this.InProductionDate;

            t.PreShipment = this.PreShipment;
            t.PreShipmentDate = this.PreShipmentDate;

            t.Cancelled = this.Cancelled;
            t.CancelledDate = this.CancelledDate;

            t.Completed = this.Completed;
            t.CompletedDate = this.CompletedDate;

            t.Shipping = this.Shipping;
            t.ShippedDate = this.ShippedDate;

            return t;
        }

        private PurchaseOrderActions GetPOAvailableActions()
        {
            PurchaseOrderActions t = new PurchaseOrderActions();

            t.OrderActive = this.Active;
            t.OrderStatus = this.Status;
            t.IsOwned = this.IsOwned;


            t.RequestShippingQuotation = !this.LogisticsQuotation.Requested;

            t.Reset = !this.IsLocked;

            if (this.Completed || this.Cancelled)
            {
                t.Reset = false;

                return t;
            }

            if (!this.Submitted)
            {
                t.Reset = false;
                t.Resend = false;
                t.Submit = t.Cancel = true;
                return t;
            }

            if (!this.Accepted && !this.Rejected)
            {
                // Pending
                t.Accept = t.Reject = true;
                return t;
            }

            t.Resend = true;
            if (!this.InProduction && this.Accepted)
            {
                t.SetProductionStatus = true;
                return t;
            }

            if (this.InProduction && !this.PreShipment)
            {
                t.SetPreShippingStatus = true;
                return t;
            }

            if (!this.Shipping && this.PreShipment)
            {
                t.SetShippingStatus = true;
                return t;
            }

            if (this.Shipping && !this.Completed)
            {
                t.Complete = true;
                return t;
            }

            return t;
        }

        [JsonIgnore]
        public OrganisationTypeEnum OrganisationType { get; set; }

        public int NoSchedules { get; set; }
        public int NoShipments { get; set; }

        public int ShipmentsQuotationRequested { get; set; }
        public int ShipmentsBooked { get; set; }
        public int ShipmentsInTransit { get; set; }
        public int ShipmentsDelivered { get; set; }

        public int AirShipments { get; set; }
        public int SeaShipments { get; set; }
        public int RoadShipments { get; set; }
        public int RailShipments { get; set; }

        public OrderShipmentStatusEnum OrderShipmentStatus { get; set; }

        public List<PurchaseOrderScheduleInformationResource> Schedules { get; set; }
        public List<PurchaseOrderShipmentInformationResource> Shipments { get; set; }
        public int Attachments { get; set; }

    }


    public class PurchaseOrderScheduleInformationResource
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid ScheduleId { get; set; }
        public DateTime? ConfirmedGoodsReadyDate { get; set; }
        public string Description { get; set; }
        public int ItemQty { get; set; }
        public string Supplier { get; set; }
        public Address PlaceOfLoading { get; set; }
        public Total Value { get; set; }
    }

    public class PurchaseOrderShipmentInformationResource
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid FreightMovementId { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }
        public bool Booked { get; set; }
        public bool InTransit { get; set; }
        public bool Delivered { get; set; }
        public bool QuotationRequested { get; set; }

        public string PortOfLoadingId { get; set; }
        public string PortOfLoading { get; set; }
        public string PortOfDischargeId { get; set; }
        public string PortOfDischarge { get; set; }

        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum Incoterms { get; set; }
        public LoadTypeEnum LoadType { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public List<string> Tags { get; set; }

        public QuotationStateEnum QuoteState { get; set; }

    }


    [TypescriptAutoGeneration]
    public class PurchaseOrderShippingQuoteStatus
    {
        [JsonProperty("Requested")]
        public bool Requested { get; set; }

        [JsonProperty("Pending")]
        public int Pending { get; set; }

        [JsonProperty("Ready")]
        public int Ready { get; set; }

        [JsonProperty("Accepted")]
        public int Accepted { get; set; }

        [JsonProperty("Expired")]
        public int Expired { get; set; }

        [JsonProperty("Rejected")]
        public int Rejected { get; set; }

    }



    [TypescriptAutoGeneration]
    public class PurchaseOrderEventResource
    {
        [JsonProperty("PurchaseOrderId")]
        public Guid PurchaseOrderId { get; set; }

        [JsonProperty("EventTime")]
        public DateTime EventTime { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [JsonProperty("ProfileImage")]
        public string ProfileImage { get; set; }
    }
}
