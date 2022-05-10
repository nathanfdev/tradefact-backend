using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class UnsassignProductRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ProductId { get; set; }
    }

    public class UnassignProductRequestValidator : AbstractValidator<UnsassignProductRequest>
    {
        public UnassignProductRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}