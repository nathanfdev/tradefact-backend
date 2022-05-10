using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Utilities.DataImport.Model
{
    public class Position
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class PortLocation
    {
        public string LocCode { get; set; }
        public string Name { get; set; }
        public string IATA { get; set; }
        public Position Position { get; set; }
        public bool Port { get; set; }
        public bool Rail { get; set; }
        public bool Road { get; set; }
        public bool Airport { get; set; }
        public object Country { get; set; }
        public bool MatchedSeaport { get; set; }
        public bool MatchedAirport { get; set; }
    }

    public class CountryData
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<PortLocation> Locations { get; set; }
    }
}
