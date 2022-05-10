using Core.Enums;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class PurchaseOrderShipmentInformationDTO
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
        public string Tags { get; set; }

        public QuotationStateEnum QuoteState { get; set; }

    }
    public class PurchaseOrderItemShipmentInformationDTO : PurchaseOrderShipmentInformationDTO
    {
        public Guid PurchaseOrderItemId { get; set; }
    }
}