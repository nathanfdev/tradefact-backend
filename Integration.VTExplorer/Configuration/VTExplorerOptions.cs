using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.VTExplorer.Configuration
{

    public class VTExplorerOptions
    {
        public bool Enabled { get; set; }
        public string ApplicationKey { get; set; }
        public bool EnableSatTracking { get; set; }
        public int Retries { get; set; }
    }

}
