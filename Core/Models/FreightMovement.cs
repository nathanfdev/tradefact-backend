using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class FreightMovement : BaseEntity<FreightMovement>
    {
        [JsonIgnore]
        public Guid CompanyId { get; set; }

        public string Name { get; set; }
        public string Reference { get; set; }
        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum LoadType { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }

        // Only for EXW
        public string PlaceOfLoadingMultiple { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public string PortOfLoadingId { get; set; }
        public string PortOfDischargeId { get; set; }
        public Guid? PlaceOfDispatchId { get; set; }

        public DateTime GoodsReady { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public int? NumberOfItems { get; set; } = 1;

        public bool Hazard { get; set; }
        public bool? InsuranceRequired { get; set; } = null;
        public bool? CustomsBrokerageRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public string Tags { get; set; }
        public string Notes { get; set; }
        public Guid? PurchaseOrderId { get; set; }

        public decimal ConsignmentQuantity { get; set; }

        public List<FCLItem> FCL { get; set; }
        public List<LCLItem> LCL { get; set; }
        public string HSCodes { get; set; }

        public Guid? SupplierId { get; set; }
        public Guid? BuyerId { get; set; }

        public List<FreightMovementItem> Items { get; set; }

        // Navigation properties
        public Location PortOfLoading { get; set; }
        public Location PortOfDischarge { get; set; }
        public Address PlaceOfLoading { get; set; }
        public Address PlaceOfDispatch { get; set; }

        [JsonIgnore]
        public Organisation Company { get; set; }

        [JsonIgnore]
        public List<QuotationRequest> QuotationRequests { get; set; } = new List<QuotationRequest>();

        [JsonIgnore]
        public List<Quotation> Quotations { get; set; }

        [JsonIgnore]
        public List<Shipment> Shipments { get; set; }
    }

}
