using Core.Enums;
using System;

namespace Core.Models
{
    public class ProductVariant : BaseEntity<ProductVariant>
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }

        public ImportProductDimensions Dimensions { get; set; }

        public string GoodsType { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public int UnitsPerPackage { get; set; }

        public string SKU { get; set; }

        public HazardContentsEnum HazardousContents { get; set; }

        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardNotes { get; set; }

        public Guid? HazardDocumentId { get; set; }
        public Document HazardDocument { get; set; }

        public string Reference { get; set; }
        public bool MagneticFieldContained { get; set; }

        public Guid SupplierId { get; set; }
        public Organisation Supplier { get; set; }

        public Product Product { get; set; }
    }
}