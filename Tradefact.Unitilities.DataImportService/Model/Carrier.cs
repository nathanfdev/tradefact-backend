using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Utilities.DataImport.Model
{
    public class CarrierDataItem
    {
        public int id { get; set; }
        public string scac { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string company { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public CountryDataItem country { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string established { get; set; }
        public int year_est { get; set; }
        public string carrier_group { get; set; }
        public int max_teu { get; set; }
        public int max_ships { get; set; }
        public string regions { get; set; }
        public string kind { get; set; }
    }

    public class CountryDataItem
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Summary
    {
        public int count { get; set; }
    }

    public class CarriersRootObject
    {
        public List<CarrierDataItem> items { get; set; }
        public Summary summary { get; set; }
    }

}
