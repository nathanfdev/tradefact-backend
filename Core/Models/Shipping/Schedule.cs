using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{

    public class Schedule
    {
        public int CarrierCode { get; set; }
        public int CarrierName { get; set; }
        
        // Voyage Number or FlightId
        public string TransitId { get; set; }
        public DateTimeOffset DepartureTime { get; set; }
        public DateTimeOffset ArrivalTime { get; set; }
        public string SCAC { get; set; }
        public Route Route { get; set; }
        public Vessel Vessel { get; set; }
        public Port POD { get; set; }
        public Port POL { get; set; }
        public string ReferenceCode { get; set; }

    }
}
