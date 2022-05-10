
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class GetProductRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ProductId { get; set; }
    }

    public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
    {
        public GetProductRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}