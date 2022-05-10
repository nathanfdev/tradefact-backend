using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public class PurchaseOrderEvent
    {
        public int EventId { get; set; }
        public PurchaseOrderEventType EventType { get; set; }
        public string Description { get; set; }
        public Guid ActionedBy { get; set; }
        public DateTime ActionDate { get; set; }
        public Guid PurchaseOrderId { get; set; }
    }

    public enum PurchaseOrderEventType
    {
        Created = 0,                 // Created not yet sent

        Submitted = 10,              // Sent not yet accepted
        Accepted = 20,               // Accepted by Supplier
        Production = 30,
        PreShipment = 40,
        Shipping = 50,
        Rejected = 60,               // Rejected by Supplier
        Cancelled = 70,              // Cancelled by Shipper
        Draft = 80,                  // Set back to draft after rejection

        Complete = 100,              // Cancelled bvy Shipper
        Archived = 110,              // Archived

        DocumentUploaded = 120,      // Document Uploaded
        CommentAdded = 130,          // Comment Added
        EmailSent = 140,

        POEdit = 200,                 // PO Edit 
        POReset = 300,                 // PO Reset 
        PODelete = 400                 // PO Reset 
    }
}
