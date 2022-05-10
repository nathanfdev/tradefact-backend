using Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{

    public class Product : BaseEntity<Product>
    {
        public string Description { get; set; }

        public ImportProductDimensions Dimensions { get; set; }

        public string GoodsType { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public decimal UnitsPerPackage { get; set; }

        // Product Codes
        public string SKU { get; set; }

        public ProductIdentifier Identifier { get; set; }

        public HazardContentsEnum HazardousContents { get; set; }

        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardNotes { get; set; }

        public Guid? HazardDocumentId { get; set; }
        public Document HazardDocument { get; set; }

        public string Reference { get; set; }
        public string LithiumBatteryPacking { get; set; }
        
        public bool MagneticFieldContained { get; set; }
        public bool Rotatable { get; set; }
        public bool Stackable { get; set; }

        public Guid CompanyId { get; set; }
        public Organisation Company { get; set; }

        public string Barcode { get; set; }

        [JsonIgnore]
        public virtual string Tags { get; set; }

        [JsonProperty("tags")]
        public List<string> TagsList => (this.Tags == null) ? new List<string>() : this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        // Inventory Managment

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public decimal StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum stock quantity
        /// </summary>
        public decimal MinStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity when admin should be notified
        /// </summary>
        public int NotifyStockQuantityBelow { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public int OrderQuantityMaximum { get; set; }
        public decimal IncomingStockQuantity { get; set; }
        public bool DataComplete { get; set; }

        // public ICollection<ProductDocument> Documents { get; set; }

        public List<ProductVariant> Variants { get; set; }
        public List<ProductSupplier> Suppliers { get; set; }
    }
}