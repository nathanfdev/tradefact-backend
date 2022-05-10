using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;

namespace Flare.Data
{
    public class Order : BaseEntity<Order>
    {
        public Guid OrganisationId { get; set; }
        public string ShipmentName { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNumber { get; set; }
        public ShipmentTypeEnum TransportMode { get; set; }
        public DateTime ShipmentDate { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public Guid DestinationAddressId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public Address DeliveryAddress { get; set; }
        public Address DestinationAddress { get; set; }
        public List<OrderLineItem> LineItems { get; set; }
    }

    public enum OrderStatus
    {
        AwaitingFulfillment = 0,
        TrackingActive = 1,
        TrackingComplete = 2
    };
}
