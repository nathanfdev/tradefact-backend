using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActivityTypeEnum
    {
        [EnumMember(Value = "1")] INFO = 1,
        [EnumMember(Value = "2")] STATUS = 2,
        [EnumMember(Value = "3")] COMMENTS = 3,
        [EnumMember(Value = "4")] UPLOAD = 4,
        [EnumMember(Value = "5")] INVITE = 5,
    }
}
