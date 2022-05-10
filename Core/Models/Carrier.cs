using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{


    public class Carrier 
    {
        public string SCAC { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string CarrierGroup { get; set; }
        public string Regions { get; set; }
        public bool Active { get; set; }
        public Track Tracking { get; set; }
        // public Track RailTrack { get; set; }

        // Navigation properties
        public ShipmentTypeEnum ShipmentTypeId { get; set; }
        public ShipmentType ShipmentType { get; set; }
    }

    public class Track
    {
        public bool Enabled { get; set; }
        public string OperatorValue { get; set; }
        public bool BillOfLading { get; set; }
        public bool Container { get; set; }
    }


    public class ShipmentType
    {
        public ShipmentTypeEnum Id { get; set; }
        public string Name { get; set; }
        public List<Carrier> Carriers { get; set; }
    }
}
