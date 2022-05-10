using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContainerTypeEnum
    {
        [EnumMember(Value = "1")] SEA = 1,
        [EnumMember(Value = "2")] AIR = 2,
        [EnumMember(Value = "3")] ROAD = 3,
        [EnumMember(Value = "4")] RAIL = 4,
        [EnumMember(Value = "5")] ALL = 5
    }

}
