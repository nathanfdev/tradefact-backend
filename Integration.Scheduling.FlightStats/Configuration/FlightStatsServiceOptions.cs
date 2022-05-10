using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.FlightStats
{
    public class FlightStatsOptions
    {
        public bool Enabled { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationKey { get; set; }
        public string CodeType { get; set; }
        public int Retries { get; set; }
    }
}
