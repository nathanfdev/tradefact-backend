using FluentValidation;

namespace Flare.Api.Model
{
    public class CreateAddressRequest
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string County { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class CreateAddressRequestValidator : AbstractValidator<CreateAddressRequest>
    {
        public CreateAddressRequestValidator()
        {
            // RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.AddressLine1).NotEmpty().Length(1, 150);
            RuleFor(x => x.City).NotEmpty().Length(3, 150);
            RuleFor(x => x.CountryCode).NotEmpty().Length(2);
        }
    }
}