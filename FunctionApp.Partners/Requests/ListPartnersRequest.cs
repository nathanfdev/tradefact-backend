using FluentValidation;

namespace FunctionApp.Partners.Requests
{
    public class ListPartnersRequest
    {
        public bool IsActive { get; set; } = true;
    }

    public class ListPartnersRequestValidator : AbstractValidator<ListPartnersRequest>
    {
    }
}