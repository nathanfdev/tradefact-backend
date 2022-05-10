using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class FreightMovementItem
    {
        public Guid FreightMovementId { get; set; }
        public Guid Id { get; set; }
        public string ContainerTypeCode { get; set; }
        public int CartonQty { get; set; }
        public string HazardCode { get; set; }
        public List<CargoItem> CargoItems { get; set; }

        // Navigation Properties
        public FreightMovement FreightMovement { get; set; }
        public ContainerType ContainerType { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderItemScheduleLineId { get; set; }
    }

}
