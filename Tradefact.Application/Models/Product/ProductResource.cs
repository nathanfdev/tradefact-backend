using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradefact.Application.Models
{
    public class ProductResource
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public string GoodsType { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public decimal UnitsPerPackage { get; set; }

        public string ProductId { get; set; }

        public string SKU { get; set; }

        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardNotes { get; set; }
        public string HazardDocumentId { get; set; }
        public bool Rotatable { get; set; }
        public bool Stackable { get; set; }

        public HazardContentsEnum HazardousContents { get; set; }
        public string Reference { get; set; }
        public bool MagneticFieldContained { get; set; }
        public string LithiumBatteryPacking { get; set; }

        public ImportProductDimensions Dimensions { get; set; }


        public int ActiveOrders { get; set; }
        public int CartonQtyOnOrder { get; set; }
        public int CartonQtyInTransit { get; set; }
        public long UnitsonOrder { get; set; }
        public long UnitsInTransit { get; set; }
        public int AvailableSuppliers { get; set; }

        public ProductIdentifier Identifier { get; set; }

        public string Barcode { get; set; }

        public decimal StockQuantity { get; set; }
        public decimal MinStockQuantity { get; set; }
        public decimal OrderQuantityMinimum { get; set; }
        public decimal IncomingStockQuantity { get; set; }
        public List<ProductSupplierResource> Suppliers { get; set; }
        public DateTime CreationDateInternal { get; set; }
        public DateTime LastModifiedOnInternal { get; set; }
        public string ThumbnailBlobUrl { get; set; }

        [JsonIgnore]
        public virtual string Tags { get; set; }

        [JsonProperty("tags")]
        public List<string> TagsList => (this.Tags == null) ? new List<string>() : this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        public bool DataComplete { get; set; }
    }

    public class SortedProductResource : ProductResource
    {
        [JsonIgnore]
        public string Search => $"{this.Name.ToLower()}{this.Nickname?.ToLower() ?? ""}{this.SKU.ToLower()}";
        public int Position(string search) => this.Search.IndexOf(search.ToLower());
    }

    public class ProductSupplierResource
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }

        public string SupplierReference { get; set; }
        public string SupplierName { get; set; }
        public List<ProductSupplierCurrency> Currencies { get; set; }
        public decimal OrderQuantityMinimum { get; set; }

        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
    }

}
