using Core.Enums;
using Core.Models;
using FluentValidation;

namespace FunctionApp.Quotations.Requests
{
    public class QuotationRequest
    {
        public DateTimeGoodsReady GoodsReadyFromAndTo { get; set; }

        public IncoTypeEnum IncoType { get; set; }

        public decimal? InsuranceValue { get; set; } = 0;

        public LoadModel Load { get; set; }

        public LoadTypeEnum LoadType { get; set; }

        public int? NumberOfItems { get; set; } = 1;

        public string PlaceOfDispatch { get; set; }

        // Only for EXW
        public string PlaceOfLoading { get; set; }

        public string PortOfDischarge { get; set; }

        public string PortOfLoading { get; set; }

        public string TargetCurrency { get; set; } = "USD";
    }

    public class QuotationRequestValidator : AbstractValidator<QuotationRequest>
    {
        public QuotationRequestValidator()
        {

            // Only for EX

            //RuleFor(x => x.PlaceOfLoading).NotNull();
            RuleFor(x => x.PortOfLoading).NotEmpty().Length(3, 150);
            RuleFor(x => x.PortOfDischarge).NotEmpty().Length(3, 150);
            RuleFor(x => x.PlaceOfDispatch).NotEmpty().Length(3, 150);

            //TODO: Lookup value
            RuleFor(x => x.TargetCurrency).NotEmpty();
        }
    }
}