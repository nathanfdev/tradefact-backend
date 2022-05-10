using Core.Enums;
using System;

namespace Core.Models
{
    public class PurchaseOrderChargeItem: BaseEntity<PurchaseOrderChargeItem>
    {
        public Guid PurchaseOrderId { get; set; }
        public string Description { get; set; }
        public ChargeItemTypeEnum Type { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Rate { get; set; }
        public Total Total { get; set; }
        public Total BaseCurrencyTotal { get; set; }
        
        public PurchaseOrder PurchaseOrder { get; set; }

        public PurchaseOrderChargeItem()
        {

        }
    }

}
