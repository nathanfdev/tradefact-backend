using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class UpdateProductSupplier
    {
        public Guid SupplierId { get; set; }
        public string SupplierReference { get; set; }
        public decimal OrderQuantityMinimum { get; set; }
        public decimal? Price { get; set; }
        public List<ProductSupplierCurrency> Currencies { get; set; }
    }
}