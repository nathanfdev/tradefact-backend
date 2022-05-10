using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NetworkConnectionTypeEnum : int
    {
        [EnumMember(Value = "0")] SELF = 0,
        [EnumMember(Value = "1")] MANAGED = 1,
        [EnumMember(Value = "2")] CONNECTED = 2
    }
}
