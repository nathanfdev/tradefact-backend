using FluentValidation;
using Newtonsoft.Json;

namespace Flare.Api.Model
{
    public class CreateOrganisationRequest
    {
        [JsonRequired]
        public string Name { get; set; }
        public string Currency { get; set; }
    }

    public class CreateCompanyRequestValidator : AbstractValidator<CreateOrganisationRequest>
    {
        public CreateCompanyRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
        }
    }
}