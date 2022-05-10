using Core.Models;
using FluentValidation;
using Newtonsoft.Json;
using System;

namespace Tradfact.Api.Requests
{
    public class CreateProductVariantRequest
    {
        public Guid SupplierId { get; set; }
        public Guid ProductId { get; set; }

        [JsonRequired]
        public string Description { get; set; }

        public ImportProductDimensions Dimensions { get; set; }

        public string GoodsType { get; set; }

        public ImportProductHandling Handling { get; set; }

        public string HsCode { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public int UnitsPerPackage { get; set; }

        [JsonRequired]
        public string SKU { get; set; }
        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardousContents { get; set; }
        public string HazardNotes { get; set; }
        public string HazardDocumentId { get; set; }

        public string Reference { get; set; }
        public bool MagneticFieldContained { get; set; }

    }

    public class CreateProductVariantRequestValidator : AbstractValidator<CreateProductVariantRequest>
    {
        public CreateProductVariantRequestValidator()
        {
            RuleFor(x => x.SupplierId).NotNull().NotEmpty();
            RuleFor(x => x.ProductId).NotNull().NotEmpty();

            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.SKU).NotEmpty().Length(3, 150);
        }
    }
}