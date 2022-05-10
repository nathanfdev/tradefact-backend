using Core.Enums;
using FluentValidation;

namespace FunctionApp.PartnerDashboard.Models
{
    public class KPIRequest
    {
        public KPIPeriod BookingsPeriod { get; set; }

        public KPIPeriod CompaniesPeriod { get; set; }

        public string PartnerId { get; set; }

        public KPIPeriod RevenuePeriod { get; set; }
    }

    public class KPIRequestValidator : AbstractValidator<KPIRequest>
    {
        public KPIRequestValidator()
        {
            RuleFor(x => x.PartnerId).NotEmpty();
            RuleFor(x => x.BookingsPeriod).NotEmpty();
            RuleFor(x => x.RevenuePeriod).NotEmpty();
            RuleFor(x => x.CompaniesPeriod).NotEmpty();
        }
    }
}