using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class CreateProductRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        public string Description { get; set; }

        public ImportProductDimensions Dimensions { get; set; }

        public string GoodsType { get; set; }

        public ImportProductHandling Handling { get; set; }

        public string HsCode { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }

        [JsonRequired]
        public string SKU { get; set; }
    }

    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.SKU).NotEmpty().Length(3, 150);
        }
    }
}