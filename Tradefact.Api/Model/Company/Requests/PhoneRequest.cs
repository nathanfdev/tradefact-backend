using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class PhoneRequest
    {
        public string CountryCode { get; set; }
        public string Number { get; set; }
        public string AreaCode { get; set; }
        public bool IsDefault { get; set; }
    }

    public class PhoneRequestValidator : AbstractValidator<PhoneRequest>
    {
        public PhoneRequestValidator()
        {
            // RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Number).NotEmpty().Length(3, 150);
        }
    }
}