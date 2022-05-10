using Core.Attributes;
using System;
using System.Collections.Generic;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    [TypescriptAutoGeneration]
    public class PurchaseOrderShipmentQuotationRequest
    {
        public DateTime? TargetDeliveryDate { get; set; }

        public List<string> HSCodes { get; set; }

        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public bool? CustomsBrokerageRequired { get; set; } = null;
        public string LogisticsNotes { get; set; }
    }
}
