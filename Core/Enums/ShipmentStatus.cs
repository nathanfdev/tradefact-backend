using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Enums
{
    public enum ShipmentAction
    {
        Suspend = 0,
        Terminate = 1,
        Reactivate = 2,
        Complete = 3,
        Archive = 4,

        Book = 10,
        // Collection 
        AssignCollectionDate = 20,
        RecordCollected = 25,
        // Shipping | Transport
        AssignTrackingInformation = 40,
        RecordInTransitToPort = 45,
        ConfirmDeparturePOL = 50,
        ConfirmArrivalPOD = 55,
        // Customs
        RecordCustomsPending = 70,
        RecordIssueAtCustoms = 75,
        // Delivery
        MarkDelivered = 100,
    }

    public enum ShipmentStatus
    {
        Active = 1,                   // Booked Active ?
        Suspended = 2,
        Complete = 3,
        Terminated = 4,
        Archived = 5
    }

    [Flags]
    public enum ShipmentStage
    {
        Booked,
        // Waiting for Collection
        AwaitingCollectionDate,       // Add Collection Date
        AwaitingCollection,           // Ready for collection
        // Shipping
        AwaitingTrackingInformation,       // IMO | MMSI & BOL | Container Information
        InTransitToPort,                   // Collection Confirmed (Via Management | API data)
        Shipping,
        DeparturePOLConfirmed,             // Departure Confirmed (Via Management | API data)
        ArrivalPODConfirmed,               // Arrival Confirmed (Via Management | API data)
        // Customs
        PendingCustoms,              // Pending Customs Clearance
        IssueAtCustoms,              // Issue At Customs
        InTransitToDestination,      // Customs Cleared
        Delivered
    }
}
