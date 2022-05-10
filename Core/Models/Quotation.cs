using Core.Enums;
using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Quotation : BaseEntity<Quotation>
    {
        public DateTimeOffset IssueDate { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public string QuoteNumber { get; set; }
        public int Revision { get; set; }
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
        public Guid ShipmentId { get; set; }

        // Cargo Type
        public LoadTypeEnum? LoadType { get; set; }

        public int PaymentTermsDays { get; set; }
        public int ValidForDays { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Margin { get; set; }

        public string PaymentTerms { get; set; }
        public string TermsAndConditions { get; set; }
        public string DetailedTermsAndConditions { get; set; }
        public string Notes { get; set; }

        public Guid FreightMovementId { get; set; }
        public Guid QuotationRequestId { get; set; }

        public string Routes { get; set; }

        public QuotationRequest QuotationRequest { get; set; }
        public FreightMovement FreightMovement { get; set; }
        public List<FreightCharge> FreightCharges { get; set; } = new List<FreightCharge>();
        public List<OriginCharge> OriginCharges { get; set; } = new List<OriginCharge>();
        public List<DestinationCharge> DestinationCharges { get; set; } = new List<DestinationCharge>();
        public List<AdditionalCharge> AdditionalCharges { get; set; } = new List<AdditionalCharge>();
        public List<Schedule> Schedules { get; set; }
        public List<Shipment> Shipments { get; set; }
    }

    public abstract class QuotationChargeItem
    {
        public QuotationChargeItem()
        {

        }

        public QuotationChargeItem(string description, int quantity, decimal unitprice, decimal taxRate, decimal discount, decimal margin, string currencyCode, decimal currencyExchangeRate, int seq)
        {
            // Seq = index, Description = item.Charge, Quantity = item.Qty, UnitPrice = item.Rate, TaxRate = item.Tax }
            this.LineId = Guid.NewGuid();

            this.Seq = seq;
            this.Description = description;

            this.Quantity = quantity;
            this.UnitPrice = unitprice;
            this.TaxRate = taxRate;
            this.Margin = margin;

            decimal tax_rate_value = (this.TaxRate / 100);
            decimal margin_rate_multiplier = 1 + (this.Margin / 100);

            decimal net_amount = Math.Round((this.Quantity * this.UnitPrice) * margin_rate_multiplier, 2, MidpointRounding.ToEven);
            decimal tax_amount = Math.Round(net_amount * tax_rate_value, 2, MidpointRounding.ToEven);

            this.BaseCurrency = new Total
            {
                CurrencyId = currencyCode,
                DiscountAmount = discount,
                NetAmount = net_amount,
                TaxAmount = tax_amount,
            };

            this.Total = new Total
            {
                CurrencyId = this.BaseCurrency.CurrencyId,
                DiscountAmount = this.BaseCurrency.DiscountAmount,
                NetAmount = this.BaseCurrency.NetAmount * currencyExchangeRate,
                TaxAmount = (this.BaseCurrency.NetAmount * currencyExchangeRate) * tax_rate_value,
            };

            this.BaseCurrency.TotalAmount = this.BaseCurrency.NetAmount + this.BaseCurrency.TaxAmount;
            this.Total.TotalAmount = this.Total.NetAmount + this.Total.TaxAmount;
        }


        public Guid QuotationId { get; set; }
        public Guid LineId { get; set; }
        public int Seq { get; set; }
        public string ServiceId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool UnitPriceIncludesTax { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Margin { get; set; }
        public Total Total { get; set; }
        public Total BaseCurrency { get; set; }
        public Quotation Quotation { get; set; }

    }

    public class FreightCharge : QuotationChargeItem
    {
        public FreightCharge()
        {

        }

        public FreightCharge(string description, int quantity, decimal unitprice, decimal taxRate, decimal discount, decimal margin, string currencyCode, decimal currencyExchangeRate, int seq) : base(description, quantity, unitprice, taxRate, discount, margin, currencyCode, currencyExchangeRate, seq)
        {
        }

    }

    public class OriginCharge : QuotationChargeItem
    {
        public OriginCharge()
        {

        }

        public OriginCharge(string description, int quantity, decimal unitprice, decimal taxRate, decimal discount, decimal margin, string currencyCode, decimal currencyExchangeRate, int seq) : base(description, quantity, unitprice, taxRate, discount, margin, currencyCode, currencyExchangeRate, seq)
        {

        }
    }

    public class DestinationCharge : QuotationChargeItem
    {
        public DestinationCharge()
        {

        }

        public DestinationCharge(string description, int quantity, decimal unitprice, decimal taxRate, decimal discount, decimal margin, string currencyCode, decimal currencyExchangeRate, int seq) : base(description, quantity, unitprice, taxRate, discount, margin, currencyCode, currencyExchangeRate, seq)
        {
        }
    }

    public class AdditionalCharge : QuotationChargeItem
    {
        public AdditionalCharge()
        {

        }

        public AdditionalCharge(string description, int quantity, decimal unitprice, decimal taxRate, decimal discount, decimal margin, string currencyCode, decimal currencyExchangeRate, int seq) : base(description, quantity, unitprice, taxRate, discount, margin, currencyCode, currencyExchangeRate, seq)
        {
        }
    }

    public class QuotationChargeType
    {
        public QuotationChargeTypeEnum QuotationChargeTypeId { get; set; }
        public string Name { get; set; }
    }

    public class Total
    {
        public string CurrencyId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
