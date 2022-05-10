using Core.Enums;
using Core.Models;
using Core.Models.Criteria;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.PurchaseOrders
{
    public class PurchaseOrderSearchCriteria : SearchCriteria
    {
        public bool Completed { get; set; }
        public bool CreatedByMe { get; set; }
        public bool GetSalesOrders { get; set; }
        public bool HasException { get; set; }
        public bool? AllNoDrafts { get; set; }
        public bool? DraftOnly { get; set; }
        public Guid? SupplierId { get; set; }
        public int? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
        public PurchaseOrderViewEnum View { get; set; } = PurchaseOrderViewEnum.DEFAULT;
    }

    public class PurchaseOrderShipmentSearchCriteria
    {
        public ShipmentTypeEnum? ShipmentMethod { get; set; }
        public string[] ScheduleId { get; set; }
    }
}
