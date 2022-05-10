using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Core.Models
{
    public class ShipmentRescheduledMail : BaseEmail
    {
        [JsonProperty("partner_name")]
        public string PartnerName { get; set; }

        [JsonProperty("tradefact_id")]
        public string TradefactId { get; set; }

        [JsonProperty("shipment_name")]
        public string ShipmentName { get; set; }

        [JsonProperty("tag_list")]
        public string TagList { get; set; }
    }
}
