using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ProductsToOrder
    {
        public Guid SupplierId { get; set; }
        public List<OrderInfo> OrderInfo { get; set; }
    }
}
