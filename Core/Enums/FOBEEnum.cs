using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IncoTypeEnum
    {
        [EnumMember(Value = "1")] FOB = 1,
        [EnumMember(Value = "2")] EXW = 2,
        [EnumMember(Value = "3")] FCA = 3,
        [EnumMember(Value = "4")] FAS = 4,
        [EnumMember(Value = "5")] CFR = 5,
        [EnumMember(Value = "6")] CIF = 6,
        [EnumMember(Value = "7")] CPT = 7,
        [EnumMember(Value = "8")] CIP = 8
    }
}

