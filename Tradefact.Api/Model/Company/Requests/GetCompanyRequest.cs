
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class GetCompanyRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }
    }

    public class GetCompanyRequestValidator : AbstractValidator<GetCompanyRequest>
    {
        public GetCompanyRequestValidator() => RuleFor(x => x.CompanyId).NotEmpty();
    }
}