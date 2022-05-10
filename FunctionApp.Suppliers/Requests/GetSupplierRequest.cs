using FluentValidation;

namespace FunctionApp.Suppliers.Requests
{
    public class GetSupplierRequest
    {
        public string SupplierId { get; set; }
    }

    public class GetSupplierRequestValidator : AbstractValidator<GetSupplierRequest>
    {
        public GetSupplierRequestValidator() => RuleFor(x => x.SupplierId).NotEmpty().Length(3, 150) ;
    }
}