using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Models.TransferMate
{
    public class ConnectionHistoryResponse
    {
        [JsonProperty("errors")]
        public List<object> Errors { get; set; }

        [JsonProperty("message")]
        public List<object> Message { get; set; }

        [JsonProperty("rows")]
        public List<Row> Rows { get; set; }
    }

    public class Row
    {
        [JsonProperty("country")]
            public string Country { get; set; }

        [JsonProperty("date")]
            public string Date { get; set; }

        [JsonProperty("ip")]
            public string Ip { get; set; }

        [JsonProperty("local")]
            public string Local { get; set; }

        [JsonProperty("uid")]
            public int Uid { get; set; }
    }
}
