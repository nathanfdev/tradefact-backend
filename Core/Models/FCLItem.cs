using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    [TypescriptAutoGeneration]
    public class FCLItem
    {
        public string ContainerType { get; set; }
        public string HazardCode { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderItemScheduleLineId { get; set; }

        public List<CargoItem> CargoItems { get; set; }
    }
}
