using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class VesselPosition
    {
        public int MMSI { get; set; }               // Maritime Mobile Service Identity number of the vessel (AIS identifier)
        public string Timestamp { get; set; }       // Date and Time (in UTC) when position was recorded by AIS
        public double Latitude { get; set; }        // Geographical latitude (WGS84)
        public double Longitude { get; set; }       // Geographical longitude (WGS84)
        public double Course { get; set; }          // Course over ground (degrees)
        public double Speed { get; set; }           // Speed over ground (knots)
        public int Heading { get; set; }            // Heading (degrees) of the vessel's hull. A value of 511 indicates there is no heading data.
        public int NAVSTAT { get; set; }            // Navigation status according to AIS Specification
        public string IMO { get; set; }             // IMO number of the vessel
        public string Name { get; set; }            // Name of the vessel
        public string Callsign { get; set; }        // Callsign of the vessel
        public int Type { get; set; }               // Type of the vessel according to AIS Specification
        public int A { get; set; }                  // Dimension (meters) from AIS GPS antenna to the Bow of the vessel
        public int B { get; set; }                  // Dimension (meters) from AIS GPS antenna to the Stern of the vessel (Vessel Length = A + B)
        public int C { get; set; }                  // Dimension (meters) from AIS GPS antenna to the Port of the vessel
        public int D { get; set; }                  // Dimension (meters) from AIS GPS antenna to the Starboard of the vessel (Vessel Width = C + D)
        public double Draught { get; set; }         // Current draught (meters) of the vessel
        public string Destination { get; set; }     // Port of destination (manually entered by the Master)
        public string ETA_AIS { get; set; }         // Estimated Time of Arrival at the port of destination (manually entered by the Master)
        public string ETA { get; set; }             // Estimated Time of Arrival in full date/time format (year is added by API system)
        public string SRC { get; set; }				// Source of AIS data - Terrestrial (TER) or Satellite (SAT)
    }
}
