using Core.Enums;

namespace Core.Dtos.PurchaseOrder
{
    public class PurchaseOrderAdditionalChargeRequest
    {
        public ChargeItemTypeEnum Type { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Rate { get; set; }
    }

    public class CreatePurchaseOrderAdditionalChargeRequest: PurchaseOrderAdditionalChargeRequest
    {

    }

    public class UpdatePurchaseOrderAdditionalChargeRequest : PurchaseOrderAdditionalChargeRequest
    {

    }

}
