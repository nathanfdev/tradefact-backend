using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class PurchaseOrderDTO
    {
        public Guid PurchaseOrderId { get; set; }
        public bool Active { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>Supplier Id</summary>
        public string Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public string Currency { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string Reference { get; set; }

        public string CreatedByName { get; set; }

        public DateTime? PurchaseOrderDate { get; set; }
        public DateTime? CreationDateInternal { get; set; }

        public DateTime? GoodsReadyDate { get; set; }
        public DateTime? TargetDeliveryDate { get; set; }
        public DateTime LastModifiedOnInternal { get; set; }

        public string IncotermsVersion { get; set; }

        public ShipmentTypeEnum? ShipmentType { get; set; }
        public IncoTypeEnum? IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum? LoadType { get; set; }
        public TransactionTypeEnum? TransactionType { get; set; }

        // Only for EXW
        public Guid? PlaceOfLoadingId { get; set; }
        public string PortOfLoadingId { get; set; }
        public string PortOfDischargeId { get; set; }
        public Guid? PlaceOfDispatchId { get; set; }

        public string Language { get; set; }
        public string PurchasingGroup { get; set; }

        public string CorrespncExternalReference { get; set; }

        public string CorrespncInternalReference { get; set; }

        public int NumberOfItems { get; set; }
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public ICollection<PurchaseOrderNote> PurchaseOrderNotes { get; set; }

        public bool? CustomsBrokerageRequired { get; set; } = null;
        // Insurance
        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public string Tags { get; set; }
        public string Containers { get; set; }
        public string HSCodes { get; set; }
        public string Product_HSCodes { get; set; }

        public string PortOfLoading { get; set; }
        public string PortOfDischarge { get; set; }

        // Place Of Loading
        public string PlaceOfLoading { get; set; }
        public string PlaceofLoading_Name { get; set; }
        public string PlaceofLoading_AddressLine1 { get; set; }
        public string PlaceofLoading_AddressLine2 { get; set; }
        public string PlaceofLoading_AddressLine3 { get; set; }
        public string PlaceofLoading_AddressLine4 { get; set; }
        public string PlaceofLoading_City { get; set; }
        public string PlaceofLoading_PostalCode { get; set; }
        public string PlaceofLoading_Country_Code { get; set; }
        public string PlaceofLoading_Country_Name { get; set; }

        public string PlaceofDispatch { get; set; }
        public string PlaceofDispatch_Name { get; set; }
        public string PlaceofDispatch_AddressLine1 { get; set; }
        public string PlaceofDispatch_AddressLine2 { get; set; }
        public string PlaceofDispatch_AddressLine3 { get; set; }
        public string PlaceofDispatch_AddressLine4 { get; set; }
        public string PlaceofDispatch_City { get; set; }
        public string PlaceofDispatch_PostalCode { get; set; }
        public string PlaceofDispatch_Country_Code { get; set; }
        public string PlaceofDispatch_Country_Name { get; set; }

        public string Client { get; set; }
        public string Provider { get; set; }

        public string Notes { get; set; }
        public string RejectedReason { get; set; }
        public bool OrderRejected { get; set; }
        public string Signature { get; set; }
        public string AdditionalInformation { get; set; }

        public DateTime? DateOfIssue { get; set; }

        public string CurrencyId { get; set; }
        public string PaymentTerms { get; set; }
        public decimal TaxRate { get; set; }

        public decimal BaseCurrency_NetAmount { get; set; }
        public decimal BaseCurrency_TaxAmount { get; set; }
        public decimal BaseCurrency_TotalAmount { get; set; }

        public decimal Items_NetAmount { get; set; }
        public decimal Items_TaxAmount { get; set; }
        public decimal Items_TotalAmount { get; set; }

        public decimal Charges_NetAmount { get; set; }
        public decimal Charges_TaxAmount { get; set; }
        public decimal Charges_TotalAmount { get; set; }

        public decimal Total_NetAmount { get; set; }
        public decimal Total_TaxAmount { get; set; }
        public decimal Total_TotalAmount { get; set; }

        public OrganisationTypeEnum OrganisationType { get; set; }

        public bool Submitted { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public bool Accepted { get; set; }
        public DateTime? AcceptedDate { get; set; }

        public bool InProduction { get; set; }
        public DateTime? InProductionDate { get; set; }

        public bool PreShipment { get; set; }
        public DateTime? PreShipmentDate { get; set; }

        public bool Shipping { get; set; }
        public DateTime? ShippedDate { get; set; }
        public bool Rejected { get; set; }
        public DateTime? RejectedDate { get; set; }

        public bool Cancelled { get; set; }
        public DateTime? CancelledDate { get; set; }

        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool Deleted { get; set; }
        public DateTime? DeletionDate { get; set; }

        public int Status { get; set; }
        public string LogisticsNotes { get; set; }

        public bool ShippingQuote_Requested { get; set; }

        public int ShippingQuote_Pending { get; set; }
        public int ShippingQuote_Ready { get; set; }
        public int ShippingQuote_Accepted { get; set; }
        public int ShippingQuote_Expired { get; set; }
        public int ShippingQuote_Rejected { get; set; }
        public string AdditionalSupplierInformation { get; set; }

        public int Attachments { get; set; }
        public bool IsLocked { get; set; }
        public bool IsOwned { get; set; } = false;

        public Guid SupplierParentId { get; set; }

        public List<PurchaseOrderScheduleInformationDTO> Schedules { get; set; }
        public List<PurchaseOrderShipmentInformationDTO> Shipments { get; set; }

        public OrderShipmentStatusEnum OrderShipmentStatus => this.GetOrderShipmentStatus();

        private OrderShipmentStatusEnum GetOrderShipmentStatus()
        {
            if (this.Shipments != null && this.Shipments.Count > 0)
            {
                int shipments_count = this.Shipments.Count;

                if (this.Shipments.Count(q => q.Booked) == shipments_count) return OrderShipmentStatusEnum.ALL_BOOKED;

                if (this.Shipments.Count(q => q.Booked || q.InTransit || q.Delivered) > 0) return OrderShipmentStatusEnum.BOOKED_READY_PENDING;

                return OrderShipmentStatusEnum.READY_PENDING_NOT_BOOKED;
            }
            return OrderShipmentStatusEnum.NONE_PENDING_READY_BOOKED;
        }

    }
}