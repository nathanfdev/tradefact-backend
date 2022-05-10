using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class CreatePartnerRequest
    {
        [JsonRequired]
        public string IsoCurrency { get; set; }

        [JsonRequired]
        public string Name { get; set; }
    }

    public class CreatePartnerRequestValidator : AbstractValidator<CreatePartnerRequest>
    {
        public CreatePartnerRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.IsoCurrency).NotEmpty().MinimumLength(3).MaximumLength(3);

            //TODO: ensure this is valid based on ISO Codes.
        }
    }
}