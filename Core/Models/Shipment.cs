using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{

    public class Shipment : BaseEntity<Shipment> {
        public Guid PartnershipId { get; set; }
        public Guid QuotationRequestId { get; set; }
        public Guid FreightMovementId { get; set; }
        public string ShipmentName { get; set; }
        public string SCAC { get; set; }
        public string BillofLadingNumber { get; set; }
        public string IMO { get; set; }
        public string VesselName { get; set; }

        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum LoadType { get; set; }

        // Status & Stage
        public ShipmentStatus Status { get; set; }
        public ShipmentStage Stage { get; set; }

        // Only for EXW
        public Guid? PlaceOfLoadingId { get; set; }
        public string PortOfLoadingId { get; set; }
        public string PortOfDischargeId { get; set; }
        public Guid? PlaceOfDispatchId { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }

        public string Tags { get; set; }
        public string Notes { get; set; }

        public bool Booked { get; set; }
        public DateTime BookedDate { get; set; }

        public bool Collected { get; set; }
        public DateTime? EstimatedCollectionDate { get; set; }
        public DateTime? CollectionDate { get; set; }

        public bool InTransit { get; set; }
        public DateTime? InTransitDate { get; set; }
        public bool EquipmentTrackAvailable { get; set; }
        public bool ShipmentTrackAvailable { get; set; }
        public bool TrackinformationAdded { get; set; }

        public bool ArrivedPOD { get; set; }
        public DateTime? ArrivedPODDate { get; set; }

        public bool DepartedPOL { get; set; }
        public DateTime? DepartedPOLDate { get; set; }


        public bool InCustoms { get; set; }
        public bool IssueAtCustoms { get; set; }
        public DateTime? IssueAtCustomsDate { get; set; }
        public bool IssueAtCustomCleared { get; set; }
        public bool CustomsClearence { get; set; }
        public DateTime? CustomsClearenceDate { get; set; }

        public bool Delivered { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public bool IsRescheduled { get; set; }
        public int Rescheduled { get; set; }
        public DateTime? LastRescheduleTime { get; set; }

        public ICollection<EquipmentItem> Equipment { get; set; }
        public ICollection<ShipmentDocument> Documents { get; set; }
        public ICollection<ShipmentEvent> TrackingEvents { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        // public Partner Partner { get; set; }

        // Navigation properties
        public Location PortOfLoading { get; set; }
        public Location PortOfDischarge { get; set; }
        public Address PlaceOfLoading { get; set; }
        public Address PlaceOfDispatch { get; set; }
        public string Route { get; set; }
        public FreightMovement FreightMovement { get; set; }
        public QuotationRequest QuotationRequest { get; set; }
        public Partnership Partnership { get; set; }

    }
}
