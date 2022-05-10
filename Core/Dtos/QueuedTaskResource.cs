using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    public class QueuedTaskResource
    {
        public string Id { get; set; }
        public QueuedTaskStatus Status { get; set; }
        public string Description { get; set; }
        public string Result { get; set; }
    }

}
