using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class VerifyBankRequest
    {
        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("bic")]
        public string Bic { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("natid")]
        public int Natid { get; set; }
    }
}
