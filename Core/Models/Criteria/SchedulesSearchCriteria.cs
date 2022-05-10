using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Models.Criteria
{
    public class SchedulesSearchCriteria
    {
        // public bool Completed { get; set; }
        // public bool CreatedByMe { get; set; }
        public string CountryCode { get; set; }
        // public ShipmentStatus? ShipmentStatus { get; set; }
        public Guid? SupplierId { get; set; }
        public int? Status { get; set; }
        public List<Guid> Schedules { get; set; }
        public string Search { get; set; }

    }

}
