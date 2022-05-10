
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class DeactivateProductRequest
    {
        [JsonRequired]
        public string ProductId { get; set; }
    }

    public class DeactivateProductRequestValidator : AbstractValidator<DeactivateProductRequest>
    {
        public DeactivateProductRequestValidator() => RuleFor(x => x.ProductId).NotEmpty();
    }
}