using System.Collections.Generic;
using Core.Enums;
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class AssignQuotationRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        public DateTimeGoodsReady GoodsReadyFromAndTo { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }

        [JsonRequired]
        public IncoTypeEnum IncoType { get; set; }

        public decimal? InsuranceValue { get; set; } = 0;

        [JsonRequired]
        public string IsoCurrency { get; set; }

        [JsonRequired]
        public List<LineItemModel> LineItems { get; set; } = new List<LineItemModel>();

        public LoadModel Load { get; set; }

        public LoadTypeEnum LoadType { get; set; }

        public int? NumberOfItems { get; set; } = 1;

        [JsonRequired]
        public string PlaceOfDispatch { get; set; }

        // Only for EXW
        public string PlaceOfLoading { get; set; }

        public string PortOfDischarge { get; set; }

        [JsonRequired]
        public string PortOfLoading { get; set; }

        //[JsonProperty(PropertyName = "isoCurrencyConverted", Order = 4)]
        //public string CurrencyConverted { get; set; }
        public string Supplier { get; set; }

        public string TargetCurrency { get; set; } = "USD";

        [JsonRequired]
        public decimal TotalCost { get; set; }

        //[JsonProperty(PropertyName = "totalCostConverted", Order = 5)]
        //public decimal TotalCostConverted { get; set; }
        [JsonRequired]
        public int TransitTime { get; set; }
    }

    public class AssignQuotationRequestValidator : AbstractValidator<AssignQuotationRequest>
    {
        public AssignQuotationRequestValidator()
        {

            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.ImportId).NotEmpty();

            RuleFor(x => x.IsoCurrency).NotEmpty().Length(3);
            RuleFor(x => x.IncoType).NotEmpty();
            RuleFor(x => x.LineItems).NotEmpty();
            RuleFor(x => x.PlaceOfDispatch).NotEmpty();
            RuleFor(x => x.PortOfLoading).NotEmpty().Length(3, 150);
            RuleFor(x => x.TotalCost).NotEmpty().GreaterThan(0);
            RuleFor(x => x.TransitTime).NotEmpty();
        }
    }
}
