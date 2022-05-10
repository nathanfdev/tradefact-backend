using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class UpdatePartnerRequest : CosmosItem<UpdatePartnerRequest>
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class UpdatePartnerRequestValidator : AbstractValidator<UpdatePartnerRequest>
    {
        public UpdatePartnerRequestValidator()
        {
            RuleFor(x => x.PartnerId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.ETag).NotEmpty();
        }
    }
}