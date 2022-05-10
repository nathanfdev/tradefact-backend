using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Criteria
{
    public abstract class SearchCriteria
    {
        public ShipmentTypeEnum? ShipmentMethod { get; set; }
        public IncoTypeEnum? IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum? LoadType { get; set; }
        public string Search { get; set; }

        //public string ShipmentName { get; set; }
        //public string ShipmentReference { get; set; }
        //public string TradefactId { get; set; }
        //public string PlaceofLoading { get; set; }
        //public string PlaceofDispatch { get; set; }
        //public string PortofLoading { get; set; }
        //public string PortofDispatch { get; set; }
        //public string ProductName { get; set; }
        //public string SKU { get; set; }

        public bool? ActiveOnly { get; set; }
    }
}
