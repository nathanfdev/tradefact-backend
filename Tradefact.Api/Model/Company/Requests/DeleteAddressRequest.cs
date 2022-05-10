using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class DeleteAddressRequest
    {
        [JsonRequired]
        public string AddressId { get; set; }

        [JsonRequired]
        public string CompanyId { get; set; }
    }

    public class DeleteAddressRequestValidator : AbstractValidator<DeleteAddressRequest>
    {
        public DeleteAddressRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.AddressId).NotEmpty();
        }
    }
}