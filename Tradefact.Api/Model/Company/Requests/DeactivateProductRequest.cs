
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
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