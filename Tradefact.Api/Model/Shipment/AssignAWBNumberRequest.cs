using Core.Attributes;
using FluentValidation;
using System;

namespace Tradefact.Api.Model.Shipment
{
    [TypescriptAutoGeneration]
    public class AssignAWBNumberRequest
    {
        public string ShipmentId { get; set; }
        public string AWBPrefix { get; set; }
        public string AWBSerialNo { get; set; }
    }

    public class AssignAWBNumberRequestValidator : AbstractValidator<AssignAWBNumberRequest>
    {
        public AssignAWBNumberRequestValidator()
        {

            RuleFor(x => x.ShipmentId)
                .NotEmpty().WithMessage("Shipment Id is required")
                .Must(BeAValidGuid).WithMessage("Shipment Id format is invalid"); ;

            RuleFor(x => x.AWBPrefix)
                    .NotEmpty().WithMessage("AWB Prefix required")
                    .Must(x => x.Length == 3).WithMessage("AWB Prefix should be 3 chars")
                    .Custom((x, context) =>
                    {
                        if ((!(int.TryParse(x, out int value)) || value <= 0))
                        {
                            context.AddFailure($"{x} is not a valid AWB Prefix");
                        }
                    });

            RuleFor(x => x.AWBSerialNo).NotEmpty().WithMessage("AWB Serial No required")
                    .Must(x => x.Length == 8).WithMessage("AWB Serial No should be 8 chars")
                    .Custom((x, context) =>
                    {
                        if ((!(int.TryParse(x.Substring(0,7), out int value)) || value <= 0))
                        {
                            context.AddFailure($"{x} is not a valid AWB Serial No");
                        } else
                        {
                            string mod = (value % 7).ToString();
                            string checkDigit = x.Substring(x.Length - 1, 1);
                            if (checkDigit != mod )
                            {
                                context.AddFailure($"{x} is not a valid AWB Serial No - check digit invalid");
                            }
                        }
                    });


        }
        private bool BeAValidGuid(string unValidatedGuid)
        {
            try
            {
                if (!String.IsNullOrEmpty(unValidatedGuid))
                {
                    return (Guid.TryParse(unValidatedGuid, out Guid _));
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
