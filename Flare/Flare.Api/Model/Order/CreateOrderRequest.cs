using Core.Enums;
using System;
using System.Collections.Generic;

namespace Flare.Api.Model
{
    public class CreateOrderRequest
    {
        public string ShipmentName { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNumber { get; set; }
        public ShipmentTypeEnum TransportMode { get; set; }
        public DateTime ShipmentDate { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public Guid DestinationAddressId { get; set; }
        public List<CreateOrderLineRequest> LineItems { get; set; }
    }
}
