using Core.Models;
using Newtonsoft.Json;

namespace Tradefact.Application.Models
{
    public abstract class QuotationChargeItemResource
    {
        public int Seq { get; set; }
        public string ServiceId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        [JsonIgnore]
        public bool UnitPriceIncludesTax { get; set; }
        public decimal TaxRate { get; set; }
        public Total Total { get; set; }
        public Total BaseCurrency { get; set; }
    }

    public class FreightChargeResource : QuotationChargeItemResource
    {

    }

    public class OriginChargeResource : QuotationChargeItemResource
    {

    }

    public class DestinationChargeResource : QuotationChargeItemResource
    {

    }

    public class AdditionalChargeResource : QuotationChargeItemResource
    {

    }

    public class Totalx
    {
        public string CurrencyId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
