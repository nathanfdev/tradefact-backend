using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class AssignProductRequest
    {
        public string CompanyId { get; set; }

        public string Description { get; set; }

        public string HsCode { get; set; }

        public string ImportId { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public int Quantity { get; set; }

        public string SKU { get; set; }
    }

    public class AssignProductRequestValidator : AbstractValidator<AssignProductRequest>
    {
        public AssignProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0);
        }
    }
}