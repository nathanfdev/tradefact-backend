using Newtonsoft.Json;
using System;

namespace Core.Models.TransferMate
{
    public class GetRatesResponse
    {
        [JsonProperty("ask")]
            public string Ask { get; set; }

        [JsonProperty("ask_amount_from")]
            public string AskAmountFrom { get; set; }

        [JsonProperty("ask_amount_to")]
            public string AskAmountTo { get; set; }

        [JsonProperty("ask_dt")]
            public DateTimeOffset AskDt { get; set; }

        [JsonProperty("bid")]
            public string Bid { get; set; }

        [JsonProperty("bid_amount_from")]
            public string BidAmountFrom { get; set; }

        [JsonProperty("bid_amount_to")]
            public string BidAmountTo { get; set; }

        [JsonProperty("bid_dt")]
            public DateTimeOffset BidDt { get; set; }

        [JsonProperty("curr")]
            public string Currency { get; set; }

        [JsonProperty("mid")]
            public string Mid { get; set; }

        [JsonProperty("mid_amount_from")]
            public string MidAmountFrom { get; set; }

        [JsonProperty("mid_amount_to")]
            public string MidAmountTo { get; set; }

        [JsonProperty("mid_dt")]
            public DateTimeOffset MidDt { get; set; }

        [JsonProperty("tm_gmt_time")]
            public DateTimeOffset TmGmtTime { get; set; }
    }
}
