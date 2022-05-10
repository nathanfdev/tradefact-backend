using System;
using System.Collections.Generic;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class Container
    {
        public List<Event> Events { get; set; }
        public string ContainerNumber { get; set; }
        public ContainerType ContainerType { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ATD { get; set; }
    }

}
