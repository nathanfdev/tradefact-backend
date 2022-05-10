using Core.Attributes;
using Newtonsoft.Json;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public partial class PurchaseOrderTimeline
    {
        //Draft = 0,                   // Created not yet sent
        //Pending = 10,                // Sent not yet accepted
        //Accepted = 20,               // Accepted by Supplier
        //Production = 30,             
        //PreShipment = 40,
        //Shipping = 50,
        //Rejected = 60,               // Rejected by Supplier
        //Cancelled = 70,              // Cancelled by Shipper

        //Complete = 100,              // Cancelled bvy Shipper
        //Archived = 110               // Archived

        [JsonProperty("Submitted")]
        public bool Submitted { get; set; }
        [JsonProperty("SubmittedDate")]
        public DateTime? SubmittedDate { get; set; }

        [JsonProperty("Accepted")]
        public bool Accepted { get; set; }
        [JsonProperty("AcceptedDate")]
        public DateTime? AcceptedDate { get; set; }

        [JsonProperty("InProduction")]
        public bool InProduction { get; set; }
        [JsonProperty("InProductionDate")]
        public DateTime? InProductionDate { get; set; }

        [JsonProperty("PreShipment")]
        public bool PreShipment { get; set; }
        [JsonProperty("PreShipmentDate")]
        public DateTime? PreShipmentDate { get; set; }

        [JsonProperty("Shipped")]
        public bool Shipping { get; set; }
        [JsonProperty("ShippedDate")]
        public DateTime? ShippedDate { get; set; }
        [JsonProperty("Rejected")]
        public bool Rejected { get; set; }
        [JsonProperty("RejectedDate")]
        public DateTime? RejectedDate { get; set; }

        [JsonProperty("Cancelled")]
        public bool Cancelled { get; set; }
        [JsonProperty("CancelledDate")]
        public DateTime? CancelledDate { get; set; }

        [JsonProperty("Completed")]
        public bool Completed { get; set; }
        [JsonProperty("CompletedDate")]
        public DateTime? CompletedDate { get; set; }

    }

}
