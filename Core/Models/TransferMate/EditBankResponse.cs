using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class EditBankResponse
    {
        [JsonProperty("account_description")]
        public object AccountDescription { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("actions")]
        public string Actions { get; set; }

        [JsonProperty("active")]
        public int Active { get; set; }

        [JsonProperty("bank_details")]
        public string BankDetails { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("direct_debit")]
        public int DirectDebit { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("is_default")]
        public string IsDefault { get; set; }

        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }

        [JsonProperty("wire_transfer")]
        public int WireTransfer { get; set; }
    }
}