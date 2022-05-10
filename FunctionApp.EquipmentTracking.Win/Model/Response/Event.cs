using System;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class Event
    {
        public Activity Activity { get; set; }
        public string State { get; set; }
        public Location Location { get; set; }
        public DateTime? DateTime { get; set; }
        public Transport Transport { get; set; }
        public Locations Locations { get; set; }
    }

}
