using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChargeItemTypeEnum
    {
        [EnumMember(Value = "1")] TAX = 1,
        [EnumMember(Value = "2")] VALUE = 2
    }

}
