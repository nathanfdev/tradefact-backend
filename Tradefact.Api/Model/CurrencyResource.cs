using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model
{
    public class CurrencyResource
    {
        public string Code { get; set; }
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public List<CurrencyCountryResource> Countries { get; set; }

        public string SearchKeywords { get; set; }
    }

    public class CurrencyCountryResource
    {
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public string Name { get; set; }
    }

}
