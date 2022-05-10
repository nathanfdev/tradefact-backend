using FluentValidation;

namespace FunctionApp.PartnerDashboard.Requests
{
    public class BillingsRequest
    {
        public string PartnerId { get; set; }
    }

    public class BillingsRequestValidator : AbstractValidator<BillingsRequest>
    {
        public BillingsRequestValidator() => RuleFor(x => x.PartnerId).NotEmpty();
    }
}