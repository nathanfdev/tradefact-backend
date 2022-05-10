using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Activity : BaseEntity<Activity>
    {
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
        public ActivityTypeEnum Type { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Description { get; set; }
        public ActivityEntityTypeEnum Entity { get; set; }
        public string Data { get; set; }
    }
}
