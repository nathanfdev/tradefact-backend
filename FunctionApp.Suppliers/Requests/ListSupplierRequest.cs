using FluentValidation;

namespace FunctionApp.Suppliers.Requests
{
    public class ListSuppliersRequest
    {
    }

    public class ListSuppliersRequestValidator : AbstractValidator<ListSuppliersRequest>
    {
        public ListSuppliersRequestValidator() { }
    }
}