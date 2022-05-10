using System;

namespace Tradefact.Application.Models
{
    public class CargoItemResource
    {
        public Guid ProductId { get; set; }
        public string HsCode { get; set; }
        public string SKU { get; set; }

        public decimal Qty { get; set; }
        public int CartonQty { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public string UOL { get; set; }

        public decimal Weight { get; set; }
        public string UOW { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public string PlaceOfLoadingName { get; set; }
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }

        public ProductResource Product { get; set; }
    }

}
