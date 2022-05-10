using Core.Enums;
using Core.Models;
using System;
using Tradefact.Application.Models;

namespace Tradefact.Api.Responses
{
    public class FreightMovementInfo
    {
        public Guid Id { get; set; }
        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum LoadType { get; set; }

        public string Reference { get; set; }
        public DateTime GoodsReady { get; set; }

        public AddressResource PlaceOfLoading { get; set; }
        public AddressResource PortOfLoading { get; set; }

        public Location PortOfDischarge { get; set; }
        public Location PlaceOfDispatch { get; set; }
        public DateTime DeliveryDate { get; set; }

    }

    public class QuotationRequestInfo_old
    {
        public Guid Id { get; set; }
        public DateTime GoodsReady { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PortOfLoading { get; set; }
        public string PlaceOfDischarge { get; set; }
        public string PortOfDischarge { get; set; }
        public int Incoterms { get; set; }
        public int ShipmentType { get; set; }
        public int LoadType { get; set; }
        public string Reference { get; set; }
        public string Partner { get; set; }
        public string Client { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
        public DateTime Submitted { get; set; }

    }

}
