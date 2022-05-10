
using Core.Attributes;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradfact.Api.Requests
{


    [TypescriptAutoGeneration]
    public class FreightMovementRequest
    {
        public Guid Id { get; set; }

        // BASIC
        public string Name { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }
        public ShipmentTypeEnum ShipmentMethod { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }

        // ORIGIN
        public string PlaceOfLoading { get; set; }
        public string PortOfLoading { get; set; }
        public DateTime GoodsReady { get; set; }

        // DESTINATION
        public string PortOfDischarge { get; set; }
        public string PlaceOfDispatch { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // CARGO
        public LoadTypeEnum LoadType { get; set; }

        public List<FCLItem> FCL { get; set; }
        public List<LCLItem> LCL { get; set; }
        public List<string> HSCodes { get; set; }

        public List<Guid> AttachedSchedules { get; set; }

        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public bool? CustomsBrokerageRequired { get; set; } = null;
        public int? NumberOfItems { get; set; } = 1;

        public Guid? SupplierId { get; set; }
        public Guid? BuyerId { get; set; }

        public List<string> Tags { get; set; }

        public string Notes { get; set; }

        public List<Recipient> ForwarderList { get; set; } = null;

        public string TagList => (Tags != null && Tags.Any()) ? String.Join(',', Tags.ToArray()) : null;
        public string HsCodeList => (HSCodes != null && HSCodes.Count > 0) ? String.Join(',', HSCodes.ToArray()) : null;

    }

}