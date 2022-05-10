
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
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