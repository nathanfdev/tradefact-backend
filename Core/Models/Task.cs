using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class QueuedTask : BaseEntity<QueuedTask>
    {
        public QueuedTaskStatus Status { get; set; }
        public string Description { get; set; }
        public string Payload { get; set; }
        public string Result { get; set; }
    }
}
