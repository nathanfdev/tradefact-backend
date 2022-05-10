using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoadTypeEnum
    {
        [EnumMember(Value = "0")] PENDING = 0,
        [EnumMember(Value = "1")] FCL = 1,
        [EnumMember(Value = "2")] LCL = 2,
        [EnumMember(Value = "3")] FTL = 3,
        [EnumMember(Value = "4")] LTL = 4
    }
}