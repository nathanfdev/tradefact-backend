using System;

namespace Core.Models
{
    public class ProductSupplierCurrency : BaseEntity<ProductSupplierCurrency>
    {
        public Guid ProductSupplierId { get; set; }
        public string CurrencyCode { get;set; }
        public decimal? Price { get; set; }
    }
}