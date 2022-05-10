using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.CargoSmart
{
    public class CargoSmartOptions
    {
        public bool Enabled { get; set; }
        public string ApplicationKey { get; set; }
        public bool EnableNearbySchedules { get; set; }
        public int SearchDuration { get; set; }
        public int Retries { get; set; }
    }
}
