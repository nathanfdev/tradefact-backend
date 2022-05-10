using System;

namespace Core.Models.External
{
    public class ExternalPurchaseOrderLineItem : BaseEntity<ExternalPurchaseOrderLineItem>
    {
        public Guid ExternalPurchaseOrderId { get; set; }
        public string LineItemID { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string SupplierReference { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string TaxType { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineAmount { get; set; }
        public string Tags { get; set; }
        public ExternalPurchaseOrder ExternalPurchaseOrder { get; set; }
    }
}
