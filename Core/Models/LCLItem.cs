using Core.Attributes;
using System;

namespace Core.Models
{
    [TypescriptAutoGeneration]
    public class LCLItem: CargoItem
    {
        public int CartonQty { get; set; }
        public string HazardCode { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderItemScheduleLineId { get; set; }
    }
}
