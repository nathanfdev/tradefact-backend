using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Location
    {
        public string LocCode { get; set; }
        public string Name { get; set; }
        public string IATA { get; set; }
        public GeographicPosition Position { get; set; }
        public bool Port { get; set; }
        public bool Rail { get; set; }
        public bool Road { get; set; }
        public bool Airport { get; set; }
        public string CountryCode { get; set; }
        public bool IsGeneric { get; set; }
        public Country Country { get; set; }
    }

    public class GeographicPosition
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

}
