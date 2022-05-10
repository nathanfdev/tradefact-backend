using Core.Attributes;
using Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models.Shipment
{
    [TypescriptAutoGeneration]
    public class ShipmentResource
    {
    }

    [TypescriptAutoGeneration]
    public class TimeLine
    {
        [JsonProperty(Order = 1)] 
        public BookingStage Booked { get; set; } = new BookingStage { State = EventClassifier.Planned };
        [JsonProperty(Order = 2)] 
        public CollectionStage Collection { get; set; } = new CollectionStage { State = EventClassifier.Planned };
        [JsonProperty(Order = 3)] 
        public InTransitStage InTransit { get; set; } = new InTransitStage { State = EventClassifier.Planned };
        [JsonProperty(Order = 4)] 
        public CustomsStage Customs { get; set; } = new CustomsStage { State = EventClassifier.Planned };
        [JsonProperty(Order = 5)] 
        public DeliveryStage Delivery { get; set; } = new DeliveryStage { State = EventClassifier.Planned };
    }


    [TypescriptAutoGeneration]
    public abstract class TimeLineStage
    {
        public EventClassifier State { get; set; }
        public DateTime? DateTime { get; set; }
    }

    [TypescriptAutoGeneration]
    public class BookingStage: TimeLineStage
    {
        public DateTime GoodsReady { get; set; }
        public DateTime? EstimatedCollection { get; set; }
    }

    [TypescriptAutoGeneration]
    public class CollectionStage : TimeLineStage
    {

    }

    [TypescriptAutoGeneration]
        public class InTransitStage : TimeLineStage
    {
        public bool InTransitToPOL { get; set; } = false;
        public bool Shipping { get; set; } = false;
        public bool ArrivedPOD { get; set; } = false;
        public bool TrackingAvailable { get; set; } = false;
    }

    [TypescriptAutoGeneration]
    public class ArrivedAtPortStage : TimeLineStage
    {
    }

    [TypescriptAutoGeneration]
    public class CustomsStage : TimeLineStage
    {
        public bool IssueAtCustoms { get; set; } = false;
        public bool CustomsCleared { get; set; } = false;
    }

    [TypescriptAutoGeneration]
    public class DeliveryStage : TimeLineStage
    {
    }

    [TypescriptAutoGeneration]
    public partial class Event
    {
        [JsonProperty("Activity")]
        public Activity Activity { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Location")]
        public Location Location { get; set; }

        [JsonProperty("DateTime")]
        public DateTimeOffset DateTime { get; set; }

        [JsonProperty("Transport")]
        public Transport Transport { get; set; }
    }

    [TypescriptAutoGeneration]
    public partial class Activity
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }

    [TypescriptAutoGeneration]
        public partial class Location
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
    }

    [TypescriptAutoGeneration]
    public partial class Transport
    {
        [JsonProperty("IMONumber")]
        public object ImoNumber { get; set; }

        [JsonProperty("Carrier")]
        public string Carrier { get; set; }

        [JsonProperty("Vessel")]
        public string Vessel { get; set; }

        [JsonProperty("Voyage")]
        public string Voyage { get; set; }
    }

}
