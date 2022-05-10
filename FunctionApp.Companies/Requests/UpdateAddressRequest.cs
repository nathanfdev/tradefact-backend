using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class UpdateAddressRequest : CosmosItem<UpdateAddressRequest>
    {
        [JsonRequired]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        [JsonRequired]
        public string City { get; set; }

        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string Country { get; set; }

        public string County { get; set; }

        public bool IsDefault { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }
    }

    public class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
    {
        public UpdateAddressRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.AddressLine1).NotEmpty().Length(3, 150);
            RuleFor(x => x.City).NotEmpty().Length(3, 150);
            RuleFor(x => x.Country).NotEmpty().Length(3, 150);
        }
    }
}