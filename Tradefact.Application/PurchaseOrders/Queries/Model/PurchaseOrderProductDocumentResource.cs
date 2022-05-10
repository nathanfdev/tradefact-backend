using Core.Attributes;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderProductDocumentResource
    {
        public Guid ProductId { get; set; }
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public int DocumentType { get; set; }
        public bool IsRichText { get; set; }
        public bool Attached { get; set; }
        public bool IsProductVariantDocument { get; set; }
        public bool Owner { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;
        public string UploadedBy { get; set; }
    }
}
