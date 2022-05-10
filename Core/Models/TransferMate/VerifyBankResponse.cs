using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class VerifyBankResponse
    {
        [JsonProperty("bankid")]
        public int Bankid { get; set; }

        [JsonProperty("bankname")]
        public string Bankname { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("unqkey")]
        public int Unqkey { get; set; }
    }
}
