using Core.Models;
using FluentValidation;

namespace FunctionApp.Suppliers.Requests
{
    public class SupplierCreateRequest
    {
        public Address Address { get; set; }

        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public string ContactTelephone { get; set; }

        public string Name { get; set; }
    }

    public class SupplierCreateRequestValidator : AbstractValidator<SupplierCreateRequest>
    {
        public SupplierCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.ContactName).NotEmpty().Length(3, 150);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.ContactTelephone).NotEmpty();

            RuleFor(x => x.Address.AddressLine1).NotEmpty().Length(3, 150);
            RuleFor(x => x.Address.City).NotEmpty().Length(3, 150);
            RuleFor(x => x.Address.Country).NotEmpty();
        }
    }
}