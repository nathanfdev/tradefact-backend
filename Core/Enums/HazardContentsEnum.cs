using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HazardContentsEnum
    {
        [EnumMember(Value = "0")] None = 0,
        [EnumMember(Value = "1")] LithiumBattery = 1,
        [EnumMember(Value = "2")] MagneticProperties = 2,
        [EnumMember(Value = "3")] HazardousMaterials = 3
    }
}