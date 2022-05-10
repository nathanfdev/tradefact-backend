using Core.Enums;
using FluentValidation;

namespace FunctionApp.PartnerDashboard.Requests
{
    public class BookingsRequest
    {
        public string PartnerId { get; set; }

        public string SearchTerm { get; set; }

        public ImportStatus Status { get; set; }
    }

    public class BookingsRequestValidator : AbstractValidator<BookingsRequest>
    {
        public BookingsRequestValidator() => RuleFor(x => x.PartnerId).NotEmpty();
    }
}