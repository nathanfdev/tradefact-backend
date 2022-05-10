using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class UpdateProductQuantityRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ProductId { get; set; }

        [JsonRequired]
        public int Quantity { get; set; }
    }

    public class UpdateProductQuantityRequestValidator : AbstractValidator<UpdateProductQuantityRequest>
    {
        public UpdateProductQuantityRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ProductId).NotEmpty();

            RuleFor(x => x.Quantity).GreaterThan(0).LessThan(999999999);
        }
    }
}