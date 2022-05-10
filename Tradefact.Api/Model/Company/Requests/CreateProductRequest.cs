using Core.Enums;
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradfact.Api.Requests
{
    public class CreateProductRequest
    {
        [JsonRequired]

        public string Description { get; set; }

        public ImportProductDimensionsRequest Dimensions { get; set; }

        public string GoodsType { get; set; }

        public string HsCode { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public decimal UnitsPerPackage { get; set; }

        [JsonRequired]
        public string SKU { get; set; }
        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardNotes { get; set; }
        public string HazardDocumentId { get; set; }

        public HazardContentsEnum HazardousContents { get; set; }
        public string LithiumBatteryPacking { get; set; }

        public string Reference { get; set; }
        public string SupplierReference { get; set; }
        public bool? MagneticFieldContained { get; set; }
        public bool? Rotatable { get; set; }
        public bool? Stackable { get; set; }

        public ProductIdentifier Identifier { get; set; }

        public string Barcode { get; set; }

        public decimal StockQuantity { get; set; }
        public decimal MinStockQuantity { get; set; }
        public decimal IncomingStockQuantity { get; set; }
        public List<UpdateProductSupplier> Suppliers { get; set; }

        [JsonIgnore]
        public string Tags => (TagList != null && this.TagList.Any()) ? String.Join(',', this.TagList.ToArray()) : null;

        [JsonProperty("Tags")]
        public List<string> TagList { get; set; }

        public bool? DataComplete { get; set; }

    }

    public class ImportProductDimensionsRequest
    {
        public decimal? Height { get; set; }

        public decimal? Length { get; set; }

        public string Scale { get; set; }

        public decimal? Width { get; set; }
        public decimal? Weight { get; set; }

        public string weightMeasurement { get; set; }

        virtual public bool MeasurementsSet() 
        {
            if (this.Height == null || this.Length == null || this.Width  == null || this.Weight == null)
            {
                return false;
            }

            return true;
        }
    }


    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.SKU).NotEmpty().Length(3, 150);

            RuleFor(x => x.UnitsPerPackage).GreaterThan(0);

            RuleFor(x => x.Dimensions).NotNull().SetValidator(new ImportProductDimensionsValidator());
        }

        private class ImportProductDimensionsValidator : AbstractValidator<ImportProductDimensionsRequest>
        {
            public ImportProductDimensionsValidator()
            {
                RuleFor(x => x.Width).NotEmpty().GreaterThanOrEqualTo(0);
                RuleFor(x => x.Height).NotEmpty().GreaterThanOrEqualTo(0);
                RuleFor(x => x.Length).NotEmpty().GreaterThanOrEqualTo(0);
                RuleFor(x => x.Weight).NotEmpty().GreaterThanOrEqualTo(0);
                RuleFor(x => x.weightMeasurement).NotEmpty().Length(1, 150);
            }
        }
    }
}