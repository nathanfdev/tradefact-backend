using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class AssignSupplierRequest
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
        public string ContactEmail { get; set; }

        [JsonRequired]
        public string ContactName { get; set; }

        [JsonRequired]
        public string ContactTelephone { get; set; }

        [JsonRequired]
        public string Country { get; set; }

        public string County { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }

        [JsonRequired]
        public string SupplierId { get; set; }
    }

    public class AssignSupplierRequestValidator : AbstractValidator<AssignSupplierRequest>
    {
        public AssignSupplierRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.ImportId).NotEmpty();
            RuleFor(x => x.SupplierId).NotEmpty();

            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.ContactName).NotEmpty().Length(3, 150);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.ContactTelephone).NotEmpty();

            RuleFor(x => x.AddressLine1).NotEmpty().Length(3, 150);
            RuleFor(x => x.City).NotEmpty().Length(3, 150);
            RuleFor(x => x.Country).NotEmpty();
        }
    }
}