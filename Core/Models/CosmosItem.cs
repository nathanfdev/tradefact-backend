using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Core.Models
{
    public interface ICosmosItem
    {
        DateTime UpsertDate
        {
            get;
            set;
        }
    }

    public abstract class CosmosItem<T>: ICosmosItem
    where T : class
    {
        [JsonProperty(PropertyName = "_eTag", Order = -996)]
        public string ETag { get; set; }

        [JsonProperty(Order = -1000)]
        [Column(Order = 1)]
        public Guid Id { get; set; }

        [JsonProperty(Order = -998)]
        [Column(Order = 2)]
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public virtual string PartitionKeyValue => Id.ToString();

        [JsonProperty(Order = -999)]
        public string Type => typeof(T).Name;

        [JsonProperty(Order = -997)]
        [Column(Order = 603)]
        public DateTime UpsertDate { get; set; }

        [JsonIgnore]
        [Column(Order = 604)]
        public byte[] Timestamp { get; set; }
    }
}