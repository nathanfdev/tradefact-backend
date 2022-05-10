using Core.Models;
using System;

namespace Tradefact.Application
{
    public class ActivityResource
    {
        public ActivityTypeEnum Type { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Description { get; set; }
        public ActivityEntityTypeEnum Entity { get; set; }
        public object Data { get; set; }
        public DateTime CreationDateInternal { get; set; }
    }
}
