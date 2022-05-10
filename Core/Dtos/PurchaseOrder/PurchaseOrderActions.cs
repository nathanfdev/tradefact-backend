using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos.PurchaseOrder
{
    public class PurchaseOrderRequest
    {
        // public Guid PurchaseOrderId { get; set; }
    }

    public class ResendPurchaseOrderRequest
    {
        public string AdditionalSupplierInformation { get; set; }
        public List<Guid> Contacts { get; set; }
    }

    public class SubmitPurchaseOrderRequest : PurchaseOrderRequest
    {
        public bool Submitted { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string AdditionalSupplierInformation { get; set; }
        public List<Guid> Contacts { get; set; }
    }

    public class AcceptPurchaseOrderRequest : PurchaseOrderRequest
    {
        public bool Accepted { get; set; }
        public DateTime? AcceptanceDate { get; set; }
    }

    public class RejectPurchaseOrderRequest : PurchaseOrderRequest
    {
        public bool Rejected { get; set; }
        public DateTime? RejectionDate { get; set; }
        public string Notes { get; set; }
    }

    public class UpdatePurchaseOrderProductionStateRequest : PurchaseOrderRequest
    {
        public bool InProduction { get; set; }
        public DateTime? ProductionStateChangeDate { get; set; }
    }

    public class UpdatePurchaseOrderPreShippingStateRequest : PurchaseOrderRequest
    {
        public bool PreProduction { get; set; }
        public DateTime? PreProductionStateChangeDate { get; set; }
    }

    public class UpdatePurchaseOrderShippingStateRequest : PurchaseOrderRequest
    {
        public bool Shipping { get; set; }
        public DateTime? ShippingStateChangeDate { get; set; }
    }

    public class CompletePurchaseOrderRequest : PurchaseOrderRequest
    {
        public bool Complete { get; set; }
        public DateTime? CompletionDate { get; set; }
    }


}
