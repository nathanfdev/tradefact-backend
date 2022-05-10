using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class AddBankRequest
    {
        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("country_id")]
        public int CountryId { get; set; }

        [JsonProperty("currency_type_id")]
        public string CurrencyTypeId { get; set; }

        [JsonProperty("iban_account_number")]
        public string IbanAccountNumber { get; set; }

        [JsonProperty("swift")]
        public int Swift { get; set; }

        [JsonProperty("transit_code")]
        public string TransitCode { get; set; }
    }
}
