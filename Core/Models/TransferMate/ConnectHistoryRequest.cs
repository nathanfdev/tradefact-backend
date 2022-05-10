using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class ConnectionHistoryRequest
    {
        [JsonProperty("date_from")]
        public string DateFrom { get; set; }

        [JsonProperty("date_to")]
        public string DateTo { get; set; }

        [JsonProperty("order_by")]
        public string OrderBy { get; set; }

        [JsonProperty("order_type")]
        public string OrderType { get; set; }

        [JsonProperty("page_no")]
        public int PageNo { get; set; }
    }
}
