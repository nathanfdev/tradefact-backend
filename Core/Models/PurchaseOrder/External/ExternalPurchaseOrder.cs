using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.External
{
    public class ExternalPurchaseOrder : BaseEntity<ExternalPurchaseOrder>    
    {
        public string ExternalID { get; set; }
        public Guid OrganisationId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Reference { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? GoodsReadyDate { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string PlaceOfIssue { get; set; }
        public string Status { get; set; }

        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Total { get; set; }
        public string PaymentTerms { get; set; }

        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Tags { get; set; }
        public List<ExternalPurchaseOrderLineItem> LineItems { get; set; } = new List<ExternalPurchaseOrderLineItem>();

        // Processing
        public DateTime Received { get; set; }
        public bool Imported { get; set; }
        public DateTime? ImportDate { get; set; }
        public Guid? ImportUserId { get; set; }
        public string ImportUserName { get; set; }
        public Guid? GenericProductId { get; set; }
        public string Source { get; set; }
    }
}
