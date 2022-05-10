using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.AWBTracking.TraxonCargoHUB.Model
{
    public class IATAInformation
    {
        public string IATACode { get; set; }
        public string AirportName { get; set; }
        public string Position_Longitude { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        public override string ToString() { 
            return $"{this.AirportName}, {this.CountryCode} [{this.IATACode}]"; 
        }
    }
}
