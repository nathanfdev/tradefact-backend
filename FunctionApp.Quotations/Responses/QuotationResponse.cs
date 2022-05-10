using System.Collections.Generic;
using Core.Models;

namespace FunctionApp.Quotations.Responses
{
    public class QuotationResponse : CosmosItem<QuotationResponse>
    {
        public string IsoCurrency { get; set; }

        public string CurrencyConverted { get; set; }

        public List<LineItemModel> LineItems { get; set; } = new List<LineItemModel>();

        public string Supplier { get; set; }

        public decimal TotalCost { get; set; }

        public decimal TotalCostConverted { get; set; }

        public int TransitTime { get; set; }
    }
}