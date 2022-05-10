using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class DeactivatePartnerRequest
    {
        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class DeactivatePartnerRequestValidator : AbstractValidator<DeactivatePartnerRequest>
    {
        public DeactivatePartnerRequestValidator() => RuleFor(x => x.PartnerId).NotNull();
    }
}