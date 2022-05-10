namespace Core.Models
{
    public class ImportProduct : CosmosItem<ImportProduct>
    {
        public string CompanyId { get; set; }

        public string Description { get; set; }

        public ImportProductDimensions Dimensions { get; set; }

        public string GoodsType { get; set; }

        public ImportProductHandling Handling { get; set; }

        public string HsCode { get; set; }

        public string ImportId { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Packing { get; set; }

        public string Weight { get; set; }
        public string WeightUOM { get; set; }

        public override string PartitionKeyValue => CompanyId;

        public int Quantity { get; set; }

        public string SKU { get; set; }
    }
}
