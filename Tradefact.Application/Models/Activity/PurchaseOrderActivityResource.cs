using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models.Activity
{
    public class PurchaseOrderActivityResource
    {
        public Guid Id { get; set; }
        public Guid? SupplierId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public PurchaseOrderStatus Status { get; set; }
    }
}
