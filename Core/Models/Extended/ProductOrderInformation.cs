using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Extended
{
    public class ProductOrderInformation: Product
    {
        public int ActiveOrders { get; set; }
        public int CartonQtyOnOrder { get; set; }
        public long CartonQtyInTransit { get; set; }
        public long UnitsonOrder { get; set; }
        public long UnitsInTransit { get; set; }
        public decimal Dimensions_Height { get; set; }
        public decimal Dimensions_Length { get; set; }
        public string Dimensions_Scale { get; set; }
        public decimal Dimensions_Width { get; set; }
        public decimal Dimensions_Weight { get; set; }
        public string Dimensions_weightMeasurement { get; set; }
        public int AvailableSuppliers { get; set; }
        public string ThumbnailBlobUrl { get; set; }
    }


    public class ProductSupplierInformation
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierReference { get; set; }
        public decimal Price { get; set; }
        public decimal OrderQuantityMinimum { get; set; }
    }

    public class ProductOrderInformationWithCount : ProductOrderInformation
    {
        public int RowCount { get; set; }
    }
}
