using System;
using System.Collections.Generic;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class PurchaseOrderWidgetResource
    {
        public List<PurchaseOrderWidgetAcceptedOrders> AcceptedOrders { get; set; }
        public int ActiveTotal { get; set; }
        public PurchaseOrderWidgetStatusInfo StatusInfo { get; set; }
    }

    public class PurchaseOrderWidgetStatusCalculations
    {
        public int Total { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; }
    }

    public class PurchaseOrderWidgetStatusInfo
    {
        public PurchaseOrderWidgetStatusCalculations Rejected { get; set; }
        public PurchaseOrderWidgetStatusCalculations Pending { get; set; }
        public PurchaseOrderWidgetStatusCalculations Accepted { get; set; }
        public PurchaseOrderWidgetStatusCalculations Shipping { get; set; }
    }

    public class PurchaseOrderWidgetAcceptedOrders
    {
        public Guid Id { get; set; }
        public string PoNumber { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public string SupplierName { get; set; }
    }
}
