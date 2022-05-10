using System;

namespace Core.Models
{
    public class EquipmentItem : CosmosItem<EquipmentItem>
    {
        public string ContainerNo { get; set; }
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
    }
}
