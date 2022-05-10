using FluentValidation;

namespace FunctionApp.Suppliers.Requests
{
    public class DeactivateSupplierRequest
    {
        public string SupplierId { get; set; }
    }

    public class DeactivateSupplierRequestValidator : AbstractValidator<DeactivateSupplierRequest>
    {
        public DeactivateSupplierRequestValidator() => RuleFor(x => x.SupplierId).NotEmpty().Length(3, 150) ;
    }
}