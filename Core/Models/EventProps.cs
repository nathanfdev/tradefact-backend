using System;

namespace Core.Models
{
    public class EventProps
    {
        public object Segment { get; set; } = null;
        public TradefactEventProps Tradefact { get; set; } = null;
    }

    public class TradefactEventProps
    {
        public string Reference { get; set; }
        public ActivityTypeEnum Type { get; set; }
        public ActivityEntityTypeEnum Entity { get; set; }
        public string CustomDescription { get; set; }
    }
}
