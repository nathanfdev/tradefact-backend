
using Core.Attributes;
using Core.Enums;
using Core.Models;
using FluentValidation;
using Integration.CargoSmart;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradfact.Api.Requests
{
    [TypescriptAutoGeneration]
    public class QuotationEdit
    {
        public Guid QuotationRequestId { get; set; }

        // CARGO
        public LoadTypeEnum? LoadType { get; set; }

        public bool IsManualRoute { get; set; }
        public List<RouteSchedule> Routes { get; set; }
        public List<QuotationCharge> FreightCharges { get; set; }
        public List<QuotationCharge> OriginCharges { get; set; }
        public List<QuotationCharge> DestinationCharges { get; set; }
        public List<QuotationCharge> AdditionalCharges { get; set; }

        public int PaymentTermsDays { get; set; }
        public int ValidForDays { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Margin { get; set; }

        public string CurrencyCode { get; set; }
        public DateTime ValidUntil { get; set; }
        public string DefaultTerms { get; set; }
        public string DetailedTerms { get; set; }
        public string Notes { get; set; }
    }

    public class QuotationCreateRequest : QuotationEdit
    {
    }

    public class QuotationEditResource : QuotationEdit
    {
    }


    public class QuotationCharge
    {
        public string Charge { get; set; }
        public int Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Tax { get; set; }
        public decimal Margin { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class QuotationChargeValidator : AbstractValidator<QuotationCharge>
    {
        public QuotationChargeValidator()
        {
            RuleFor(x => x.Qty).NotNull().GreaterThan(0);
            RuleFor(x => x.Rate).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.Margin).NotNull().GreaterThanOrEqualTo(0);
        }
    }

    public class QuotationCreateRequestValidator : AbstractValidator<QuotationCreateRequest>
    {
        public QuotationCreateRequestValidator()
        {
            RuleFor(x => x.QuotationRequestId).NotNull().NotEmpty();

            RuleFor(x => x.ValidUntil)
                .NotEmpty().WithMessage("Required field")
                .GreaterThanOrEqualTo(p => DateTime.Now.ToUniversalTime().Date).WithMessage("Valid until date must not be in the past");

            // RuleFor(x => x.ValidForDays).NotNull().NotEmpty().GreaterThan(0);

            RuleFor(x => x.CurrencyCode).NotNull().NotEmpty();
            RuleForEach(x => x.OriginCharges).SetValidator(new QuotationChargeValidator());
            RuleForEach(x => x.FreightCharges).SetValidator(new QuotationChargeValidator());
            RuleForEach(x => x.DestinationCharges).SetValidator(new QuotationChargeValidator());
            RuleForEach(x => x.AdditionalCharges).SetValidator(new QuotationChargeValidator());
        }
    }
}