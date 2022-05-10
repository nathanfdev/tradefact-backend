using Core.Models;
using FluentValidation;
using Newtonsoft.Json;
using System;

namespace Tradfact.Api.Requests
{
    public class UpdateProductVariantRequest 
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public ImportProductDimensions Dimensions { get; set; }

        public string GoodsType { get; set; }

        public ImportProductHandling Handling { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }
        public int UnitsPerPackage { get; set; }

        public string SKU { get; set; }

        public string HazardClass { get; set; }
        public string HazardDescription { get; set; }
        public string HazardousContents { get; set; }
        public string HazardNotes { get; set; }
        public string HazardDocumentId { get; set; }

        public string Reference { get; set; }
        public bool MagneticFieldContained { get; set; }

    }

    public class UpdateProductVariantRequestValidator : AbstractValidator<UpdateProductVariantRequest>
    {
        public UpdateProductVariantRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.SKU).NotEmpty().Length(3, 150);

            RuleFor(x => x.UnitsPerPackage).GreaterThan(0);
        }
    }
}