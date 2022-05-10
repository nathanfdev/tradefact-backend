using Core.Enums;
using System;

namespace Core.Models
{
    public class PurchaseOrderDocument
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid DocumentId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
        public Document Document { get; set; }

        public bool IsActive { get; set; } = true;

        public PurchaseOrderDocumentType DocumentType { get; set; }
    }

    public class PurchaseOrderDocumentResource : Document
    {
        public int DocumentType { get; set; }
    }
}
