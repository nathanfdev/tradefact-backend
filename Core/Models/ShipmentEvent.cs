using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public class ShipmentEvent
    {
        [JsonIgnore]
        public Guid ShipmentId { get; set; }

        [JsonProperty("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonProperty("equipmentItemId")]
        public string EquipmentItemId { get; set; }

        [JsonIgnore]
        public Guid EventId { get; set; }

        [JsonProperty("timeOfEvent")]
        public DateTime TimeOfEvent { get; set; }

        [JsonProperty("locationOfEvent")]
        public string LocationOfEvent { get; set; }

        [JsonProperty("voyage")]
        public string Voyage { get; set; }

        [JsonProperty("activity")]
        public string Activity { get; set; }

        [JsonProperty("Information")]
        public string Information { get; set; }

        [JsonIgnore]
        public Shipment Shipment { get; set; }
    }

}
