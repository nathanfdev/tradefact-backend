using Core.Enums;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class ImportQuotation: CosmosItem<ImportQuotation>
    {
        // Request
        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }

        public LoadTypeEnum LoadType { get; set; }

        public LoadModel FCL { get; set; }
        public LCLModel LCL { get; set; }

        public decimal? InsuranceValue { get; set; } = 0;

        public DateTimeGoodsReady GoodsReady { get; set; }

        public string CurrencyConverted { get; set; }

        public bool Hazard { get; set; }

        public string Notes { get; set; }

        public DateTime DateDue { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime SubmissionBy { get; set; }

        public Guid SubmissionTime { get; set; }

        public int? NumberOfItems { get; set; } = 1;

        public string PlaceOfDispatch { get; set; }

        // Only for EXW
        public string PlaceOfLoading { get; set; }

        public string PortOfDischarge { get; set; }

        public string PortOfLoading { get; set; }


        public string TargetCurrency { get; set; } = "USD";

        public decimal TotalCost { get; set; }

        public decimal TotalCostConverted { get; set; }

        public int TransitTime { get; set; }

        public Guid ImportId { get; set; }

        public Import Import { get; set; }
    }
}