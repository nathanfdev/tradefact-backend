using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public partial class Country
    {
        public string Name { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public List<Location> Locations { get; set; }
        public List<CurrencyCountry> Currencies { get; set; }

    }
}
