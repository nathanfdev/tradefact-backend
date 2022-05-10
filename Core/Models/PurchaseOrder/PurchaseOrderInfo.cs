using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PurchaseOrderInfo
    {
        public string Owner_Name { get; set; }
        public string Supplier_Name { get; set; }
        public string Supplier_Address1 { get; set; }
        public string Supplier_Address2 { get; set; }
        public string Supplier_Address3 { get; set; }
        public string Supplier_Address4 { get; set; }
        public string Supplier_City { get; set; }
        public string Supplier_County { get; set; }
        public string Supplier_Country { get; set; }
        public string Supplier_PostalCode { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public string Buyer_Name { get; set; }
        public string Buyer_Address1 { get; set; }
        public string Buyer_Address2 { get; set; }
        public string Buyer_Address3 { get; set; }
        public string Buyer_Address4 { get; set; }
        public string Buyer_City { get; set; }
        public string Buyer_County { get; set; }
        public string Buyer_Country { get; set; }
        public string Buyer_PostalCode { get; set; }
        public string Currency { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime? GoodsReadyDate { get; set; }
        public string Reference { get; set; }
        public string PaymentTerms { get; set; }
        public decimal Total_CurrencyId { get; set; }
        public decimal Total_NetAmount { get; set; }
        public decimal Total_TaxAmount { get; set; }
        public decimal Total_TotalAmount { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string AdditionalSupplierInformation { get; set; }
        public List<PurchaseOrderItemInfo> Items { get; set; }
        public List<PurchaseOrderItemInfo> Charges { get; set; }
        public List<PurchaseOrderItemInfo> Taxes { get; set; }
    }

    public class PurchaseOrderItemInfo
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int OrderQuantity { get; set; }
        public decimal OrderPriceUnit { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
