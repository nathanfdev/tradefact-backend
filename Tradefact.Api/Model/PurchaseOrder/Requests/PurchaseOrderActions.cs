using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.PurchaseOrder.Requests
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderAction
    {
        public Guid PurchaseOrderId { get; set; }
    }

    [TypescriptAutoGeneration]
    public class AssignGoodsReadyDateRequest: PurchaseOrderAction
    {
        public DateTime GoodsReadyDate { get; set; }
    }
}
