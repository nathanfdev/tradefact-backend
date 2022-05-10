using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class AcceptBookingRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }
    }

    public class AcceptBookingRequestValidator : AbstractValidator<AcceptBookingRequest>
    {
        public AcceptBookingRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}