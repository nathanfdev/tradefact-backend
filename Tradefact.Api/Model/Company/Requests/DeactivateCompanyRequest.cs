
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class DeactivateCompanyRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }
    }

    public class DeactivateCompanyRequestValidator : AbstractValidator<DeactivateCompanyRequest>
    {
        public DeactivateCompanyRequestValidator() => RuleFor(x => x.CompanyId).NotEmpty();
    }
}