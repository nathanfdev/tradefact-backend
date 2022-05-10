using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Core.Models
{
    public class WeeklyBriefingForwarder : BaseEmail
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("count_collections")]
        public string CollectionsCount { get; set; }

        [JsonProperty("count_arrivals")]
        public string ArrivalsCount { get; set; }

        [JsonProperty("count_arrivals_sea")]
        public string ArrivalsSeaCount { get; set; }

        [JsonProperty("count_arrivals_air")]
        public string ArrivalsAirCount { get; set; }

        [JsonProperty("count_departures")]
        public string DeparturesCount { get; set; }

        [JsonProperty("count_departures_sea")]
        public string DeparturesSeaCount { get; set; }

        [JsonProperty("count_departures_air")]
        public string DeparturesAirCount { get; set; }

        [JsonProperty("count_deliveries")]
        public string DeliveriesCount { get; set; }

        [JsonProperty("count_exception")]
        public string ExceptionsCount { get; set; }

        [JsonProperty("count_quotes_pending")]
        public string QuotesPendingCount { get; set; }

        [JsonProperty("count_shippers_active")]
        public string ShippersActiveCount { get; set; }

        [JsonProperty("conversion_rate_percent")]
        public string ConversionRatePercent { get; set; }
    }
}
