using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActivityEntityTypeEnum
    {
        [EnumMember(Value = "1")] PURCHASEORDER = 1,
        [EnumMember(Value = "2")] SHIPMENT = 2,
        [EnumMember(Value = "3")] PRODUCT = 3,
        [EnumMember(Value = "4")] NETWORK = 4,
        [EnumMember(Value = "5")] QUOTES = 5,
        [EnumMember(Value = "6")] SETTINGS = 6,
    }
}
