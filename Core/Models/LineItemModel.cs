using Core.Attributes;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    [TypescriptAutoGeneration]
    public class LineItemModel: CosmosItem<LineItemModel>
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public Guid QuotationId { get; set; }

        public ImportQuotation Quotation { get; set; }

        // public List<string> Containers { get; set; }

    }
}