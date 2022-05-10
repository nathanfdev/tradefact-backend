using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class ReactivatePartnerRequest
    {
        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class ReactivatePartnerRequestValidator : AbstractValidator<ReactivatePartnerRequest>
    {
        public ReactivatePartnerRequestValidator() => RuleFor(x => x.PartnerId).NotNull();
    }
}