namespace Core.Models
{
    public class CurrencyModel : CosmosItem<CurrencyModel>
    {
        public string Code { get; set; }

        public string Country { get; set; }

        public string IsoCurrency { get; set; }
    }
}