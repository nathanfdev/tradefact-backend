using Core.Models;
using System;
using System.Collections.Generic;

namespace Tradefact.Application.Models
{
    public class QuotationResource
    {
        public DateTimeOffset IssueDate { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public string QuoteNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactReference { get; set; }
        public string Reference { get; set; }
        public int TotalQuantity { get; set; }
        public string CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal InverseExchangeRate { get; set; }
        public decimal BaseCurrencyTotalDiscountAmount { get; set; }
        public Total Total { get; set; }
        public Total BaseCurrency { get; set; }
        public bool QuoteStatus { get; set; }
        public bool Sent { get; set; }
        public bool SentByEmail { get; set; }
        public bool Booked { get; set; }
        public string Notes { get; set; }
        public string PaymentTerms { get; set; }
        public string DefaultTerms { get; set; }
        public string DetailedTerms { get; set; }

        public int PaymentTermsDays { get; set; }
        public int ValidForDays { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Margin { get; set; }

        public List<RouteSchedule> Routes { get; set; }

        // CARGO
        public int? LoadType { get; set; }

        public List<FreightChargeResource> FreightCharges { get; set; }
        public List<OriginChargeResource> OriginCharges { get; set; }
        public List<DestinationChargeResource> DestinationCharges { get; set; }
        public List<AdditionalChargeResource> AdditionalCharges { get; set; }
    }
}
