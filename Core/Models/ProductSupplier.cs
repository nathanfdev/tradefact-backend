using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class ProductSupplier : BaseEntity<ProductSupplier>
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }

        public Product Product { get; set; }
        public Organisation Supplier { get; set; }

        public string SupplierReference { get; set; }
        public string Currency { get; set; }
        public List<ProductSupplierCurrency> Currencies { get; set; }
        public decimal OrderQuantityMinimum { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
    }
}