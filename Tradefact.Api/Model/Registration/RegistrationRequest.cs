using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Registration
{
    public class RegistrationRequest
    {
        public string CompanyLegalName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string WebAddress { get; set; }
        public string BusinessId { get; set; }
        public string CompanyBio { get; set; }
        public List<RegistrationUser> Users { get; set; } 

    }

    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.CompanyLegalName).MaximumLength(250);
            RuleFor(x => x.Country).MaximumLength(250);
            RuleFor(x => x.Address).MaximumLength(250);
            RuleFor(x => x.City).MaximumLength(250);
            RuleFor(x => x.WebAddress).MaximumLength(250);

        }
    }
}
