using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public partial class ProductFlatfileImport
    {

        [JsonProperty("UserId")]
        public string UserId { get; set; }

        [JsonProperty("BatchId")]
        public Guid BatchId { get; set; }

        [JsonProperty("ProductId")]
        public Guid? ProductId { get; set; }

        [JsonProperty("CompanyId")]
        public Guid? CompanyId { get; set; }

        [JsonProperty("SupplierId")]
        public Guid? SupplierId { get; set; }

        [JsonProperty("CorrelationId")]
        public Guid CorrelationId { get; set; }

    }
}