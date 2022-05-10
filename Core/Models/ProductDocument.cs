using Core.Enums;
using System;

namespace Core.Models
{
    public class ProductDocument
    {
        public Guid ProductId { get; set; }
        public Guid DocumentId { get; set; }

        // public Product Product { get; set; }
        public Document Document { get; set; }

        public bool IsActive { get; set; } = true;

        public ProductDocumentType DocumentType { get; set; }

        public bool IsDefaultImage { get; set; } = false;
    }
}
