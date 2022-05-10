using FluentValidation;

namespace FunctionApp.PartnerDashboard.Requests
{
    public class CompaniesRequest
    {
        public string PartnerId { get; set; }
    }

    public class CompaniesRequestValidator : AbstractValidator<CompaniesRequest>
    {
        public CompaniesRequestValidator() => RuleFor(x => x.PartnerId).NotEmpty() ;
    }
}