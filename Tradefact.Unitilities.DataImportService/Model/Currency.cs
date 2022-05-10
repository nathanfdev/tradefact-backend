using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Utilities.DataImport.Model
{
    public class ExtendedCurrencyInformation
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("currencies")]
        public List<ExtendedCurrency> Currencies { get; set; }
    }

    public partial class ExtendedCurrency
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("defaultRecipientType")]
        public string DefaultRecipientType { get; set; }

        [JsonProperty("recipientTypes")]
        public List<string> RecipientTypes { get; set; }

        [JsonProperty("hasDecimals")]
        public bool HasDecimals { get; set; }

        [JsonProperty("recipientBicRequired")]
        public bool RecipientBicRequired { get; set; }

        [JsonProperty("recipientEmailRequired")]
        public bool RecipientEmailRequired { get; set; }

        [JsonProperty("paymentReferenceAllowed")]
        public bool PaymentReferenceAllowed { get; set; }

        [JsonProperty("paymentReferenceMaxLength")]
        public long PaymentReferenceMaxLength { get; set; }

        [JsonProperty("maxSourceAmount")]
        public long MaxSourceAmount { get; set; }

        [JsonProperty("maxTargetAmount")]
        public long MaxTargetAmount { get; set; }

        [JsonProperty("minSourceAmount")]
        public long MinSourceAmount { get; set; }

        [JsonProperty("minTargetAmount")]
        public long MinTargetAmount { get; set; }

        [JsonProperty("countryKeywords", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CountryKeywords { get; set; }
        public List<CurrencyCountryInfo> Countries { get; set; } = new List<CurrencyCountryInfo>();
    }

    public class CurrencyCountryInfo
    {
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public string Name { get; set; }
    }

}
