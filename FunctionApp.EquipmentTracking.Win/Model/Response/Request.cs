using System;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class TrackRequest
    {
        public string Carrier { get; set; }
        public string SearchKey { get; set; }
        public string SearchValue { get; set; }
        public TrackRequest meta { get; set; }
        public DateTime UpdateUntil { get; set; }
        public string SearchClass { get; set; }
        public object Reference { get; set; }
        public string WebhookURL { get; set; }
    }

    public class TrackRequestMeta
    {
        public string agentid { get; set; }
        public string listName { get; set; }
        public string queryID { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime CreatedDate { get; set; }
        public object lastCheck { get; set; }
    }

}
