using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Partners.Requests
{
    public class CreateOrUpdateAddressRequest
    {
        [JsonRequired]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        [JsonRequired]
        public string City { get; set; }

        [JsonRequired]
        public string Country { get; set; }

        public string County { get; set; }

        public bool IsDefault { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string PartnerId { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }
    }

    public class CreateOrUpdateAddressRequestValidator : AbstractValidator<CreateOrUpdateAddressRequest>
    {
        public CreateOrUpdateAddressRequestValidator()
        {
            RuleFor(x => x.PartnerId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.AddressLine1).NotEmpty().Length(3, 150);
            RuleFor(x => x.City).NotEmpty().Length(3, 150);
            RuleFor(x => x.Country).NotEmpty().Length(3, 150);
        }
    }
}