using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Core.Models
{
    public class QuoteExpiredMail : BaseEmail
    {
        [JsonProperty("organisation_name")]
        public string OrganisationName { get; set; }

        [JsonProperty("quote_received_date")]
        public string QuoteReceivedDate { get; set; }

        [JsonProperty("tradefact_id")]
        public string TradefactId { get; set; }

        [JsonProperty("shipment_name")]
        public string ShipmentName { get; set; }
    }
}
