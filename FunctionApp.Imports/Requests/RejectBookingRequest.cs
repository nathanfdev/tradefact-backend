using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class RejectBookingRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }
    }

    public class RejectBookingRequestValidator : AbstractValidator<RejectBookingRequest>
    {
        public RejectBookingRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}