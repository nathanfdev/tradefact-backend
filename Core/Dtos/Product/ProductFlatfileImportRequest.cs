using Newtonsoft.Json;
using System;

namespace Core.Dtos.Product
{
    public class ProductFlatfileImportRequest
    {
        [JsonProperty("BatchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("ProductId")]
        public Guid? ProductId { get; set; }

        [JsonProperty("SupplierId")]
        public Guid? SupplierId { get; set; }
    }
}
