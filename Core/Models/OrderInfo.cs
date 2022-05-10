using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class OrderInfo
    {
        public Guid ProductId { get; set; }
        public int OrderQuantity { get; set; }
    }
}
