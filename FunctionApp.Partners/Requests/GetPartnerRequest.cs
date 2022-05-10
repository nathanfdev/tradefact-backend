using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Partners.Requests
{
    public class GetPartnerRequest
    {
        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class GetPartnerRequestValidator : AbstractValidator<GetPartnerRequest>
    {
        public GetPartnerRequestValidator() => RuleFor(x => x.PartnerId).NotNull();
    }
}