using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class ListProductsRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ListProductsRequestValidator : AbstractValidator<ListProductsRequest>
    {
        public ListProductsRequestValidator() => RuleFor(x => x.CompanyId).NotEmpty();
    }
}