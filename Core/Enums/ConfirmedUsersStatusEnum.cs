using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConfirmedUsersStatusEnum
    {
        [EnumMember(Value = "0")] PENDING = 0,
        [EnumMember(Value = "1")] ACTIVE = 1
    }
}
