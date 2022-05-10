using Core.Enums;
using Flare.Data;
using System;
using System.Collections.Generic;
using Tradefact.Application.Models;

namespace Flare.Api.Model
{
    public class OrderResource
    {
        public Guid Id { get; set; }
        public string ShipmentName { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNumber { get; set; }
        public ShipmentTypeEnum TransportMode { get; set; }
        public DateTime ShipmentDate { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public AddressResource DeliveryAddress { get; set; }
        public AddressResource DestinationAddress { get; set; }
        public List<OrderLineItemResource> LineItems { get; set; }
    }
}
