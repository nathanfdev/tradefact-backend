using System;
using System.Runtime.Serialization;

namespace Core.Models.Criteria
{
    public class ShipmentSearchCriteria: SearchCriteria
    {
        public bool Completed { get; set; }
        public bool CreatedByMe { get; set; }
        public bool HasException { get; set; }
        public ShipmentStatus? ShipmentStatus { get; set; }

        public Guid? SupplierId { get; set; }
        public Guid? BuyerId { get; set; }

        public Guid? OrganisationId { get; set; }
        public bool ShowExceptionDetailsOnly { get; set; } = false;
        public bool HasGeoCoordinates { get; set; } = false;
    }

    public enum ShipmentStatus
    {
        [EnumMember(Value = "1")] AwaitingCollection = 1,
        [EnumMember(Value = "2")] InTransitToPort = 2,
        [EnumMember(Value = "3")] Shipping = 3,
        [EnumMember(Value = "4")] PendingCustomsClearance = 4,
        [EnumMember(Value = "5")] InTransitToDestination = 5
    }
}
