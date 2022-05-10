using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OwnershipEnum
    {
        Unspecified = 0,
        Partner = 1,
        Company = 2
    }
}
