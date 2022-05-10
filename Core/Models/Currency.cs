using System.Collections.Generic;

namespace Core.Models
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public bool Active { get; set; }
        public List<CurrencyCountry> Countries { get; set; }
    }

    public class CurrencyCountry
    {
        public int CurrencyId { get; set; }
        public string CountryCode { get; set; }
        public bool Active { get; set; }
        public Country Country { get; set; }
        public Currency Currency { get; set; }
    }


}
