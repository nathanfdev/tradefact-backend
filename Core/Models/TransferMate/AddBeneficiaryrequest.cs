using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class AddBeneficiaryRequest
    {
        [JsonProperty(PropertyName = "account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("account_number_type")]
        public string AccountNumberType { get; set; }

        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        [JsonProperty("bank_branch_address")]
        public string BankBranchAddress { get; set; }

        [JsonProperty(PropertyName = "bank_branch_address_to_account_type")]
        public string BankBranchAddressToAccountType { get; set; }

        [JsonProperty(PropertyName = "beneficiary_type_id")]
        public int? BeneficiaryTypeId { get; set; }

        [JsonProperty("country_id")]
        public long CountryId { get; set; }

        [JsonProperty("currency_type_id")]
        public string CurrencyTypeId { get; set; }

        [JsonProperty("def_payment_reference")]
        public string DefPaymentReference { get; set; }

        [JsonProperty("def_transfer_reason")]
        public string DefTransferReason { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_alert_flag")]
        public long EmailAlertFlag { get; set; }

        [JsonProperty("for_further_credit_to")]
        public string ForFurtherCreditTo { get; set; }

        [JsonProperty("payee_address")]
        public string PayeeAddress { get; set; }

        [JsonProperty(PropertyName = "payee_address_to_account_type")]
        public string PayeeAddressToAccountType { get; set; }

        [JsonProperty(PropertyName = "payee_address_to_country_state")]
        public string PayeeAddressToCountryState { get; set; }

        [JsonProperty("payee_name")]
        public string PayeeName { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "swift")]
        public string Swift { get; set; }

        [JsonProperty("transit_code")]
        public long TransitCode { get; set; }

        [JsonProperty("transit_code_type")]
        public string TransitCodeType { get; set; }
    }
}