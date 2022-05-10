using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.PurchaseOrder.Requests
{
    public class CreatePurchaseOrderRequest
    {
        public Guid? SupplierId { get; set; }
        public string PONumber { get; set; }
        public string CurrencyId { get; set; }
    }

    public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
    {
        public CreatePurchaseOrderRequestValidator()
        {
            RuleFor(x => x.PONumber)
                .MaximumLength(64)
                .When(x => !string.IsNullOrEmpty(x.PONumber))
                .WithMessage("Purchase Order Number should be between 0 and 64 characters");
        }
    }

}
