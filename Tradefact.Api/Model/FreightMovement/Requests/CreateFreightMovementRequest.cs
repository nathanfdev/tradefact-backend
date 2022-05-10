
using Core.Attributes;
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    [TypescriptAutoGeneration]
    public class CreateFreightMovementRequest: FreightMovementRequest
    {
        private new string Reference { get; set; }
    }

    public class CreateFreightMovementRequestValidator : AbstractValidator<CreateFreightMovementRequest>
    {
        public CreateFreightMovementRequestValidator()
        {
            // RuleFor(x => x.Reference).NotEmpty().MinimumLength(3);

            RuleFor(x => x.LCL).Custom((list, context) => {
                if (list.Count > 1000)
                {
                    context.AddFailure("The list must contain 10 items or fewer");
                }
            });
            RuleForEach(x => x.LCL).SetValidator(new LCLValidator());
        }

        public class LCLValidator : AbstractValidator<LCLItem>
        {
            public LCLValidator()
            {
                // All your other validation rules for Guitar. eg.
                RuleFor(x => x.ProductId).NotNull();

                RuleFor(x => x.HsCode).NotNull();
                RuleFor(x => x.SKU).NotNull();

                RuleFor(x => x.Qty).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Length).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Width).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Height).GreaterThanOrEqualTo(0);
            }
        }


    }


}