using System;

namespace FunctionApp.JobEngine.Model
{
    public class ImportedProductItem
    {
        public Guid CorrelationId { get; set; }
        public int Seq { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string HsCode { get; set; }

        public Guid CompanyId { get; set; }
    }
}
