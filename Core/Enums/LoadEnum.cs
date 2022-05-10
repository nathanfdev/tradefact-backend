using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoadEnum
    {
        [EnumMember(Value = "1")] ft20 = 1,
        [EnumMember(Value = "2")] ft40 = 2,
        [EnumMember(Value = "3")] ftHHQ40 = 3,
        [EnumMember(Value = "4")] ftHQ45 = 4,
        [EnumMember(Value = "5")] Car = 5
    }
}