using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.External
{
    public class ExternalProduct
    {
        public string UserId { get; set; }
        public Guid BatchId { get; set; }
        public int SeqNo { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CompanyId { get; set; }
        // Product Identifiers
        public string SKU { get; set; }
        public ProductIdentifier Identifier { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }

        public ImportProductDimensions Dimensions { get; set; }
        public string GoodsType { get; set; }
        public string HsCode { get; set; }
        public string Packing { get; set; }
        public decimal UnitsPerPackage { get; set; }

        public HazardContentsEnum HazardousContents { get; set; }
        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardNotes { get; set; }
        public string Reference { get; set; }
        public string LithiumBatteryPacking { get; set; }
        public bool MagneticFieldContained { get; set; }
        public bool Rotatable { get; set; }
        public bool Stackable { get; set; }
        public string Barcode { get; set; }
        public string Tags { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockQuantity { get; set; }
        public int NotifyStockQuantityBelow { get; set; }
        public int OrderQuantityMaximum { get; set; }
        public int IncomingStockQuantity { get; set; }
    }
}
