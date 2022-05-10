using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.JobEngine.Model
{
    public class ImportedPOItem
    {
        public Guid CorrelationId { get; set; }
        public int Seq { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }
    }
}
