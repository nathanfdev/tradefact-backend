using Core.Models;
using System;

namespace Flare.Data
{
    public class OrderLineItem : BaseEntity<OrderLineItem>
    {
        public Guid OrderId { get; set; }
        public int LineNumber { get; set; }
        public string LabelText { get; set; }
        public Guid? DeviceId { get; set; }

        public Device Device { get; set; }
    }
}
