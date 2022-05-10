using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class TransitSchedule
    {
    }

    public class TransitScheduleRequest
    {
        public string PortOfLoading { get; set; }
        public string PortOfDischarge { get; set; }
        public List<string> Carriers { get; set; }
        public DateTime ScheduleDate { get; set; }
    }

}
