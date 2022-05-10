using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class AddBankResponse
    {
        [JsonProperty("qid")]
        public long Qid { get; set; }

        [JsonProperty("stat")]
        public string Stat { get; set; }
    }
}