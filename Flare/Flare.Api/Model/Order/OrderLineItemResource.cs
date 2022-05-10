using System;

namespace Flare.Api.Model
{
    public class OrderLineItemResource
    {
        public Guid Id { get; set; }
        public int LineNumber { get; set; }
        public string LabelText { get; set; }
        public DeviceResource Device { get; set; }
    }
}
