using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Models.ReferenceData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemCollectionTypes
    {
        Product,
        Activity
    }
}