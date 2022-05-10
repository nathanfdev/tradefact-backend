using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderShipmentStatusEnum
    {
        [EnumMember(Value = "1")] ALL_BOOKED = 1,
        [EnumMember(Value = "2")] BOOKED_READY_PENDING = 2,
        [EnumMember(Value = "3")] READY_PENDING_NOT_BOOKED = 3,
        [EnumMember(Value = "4")] NONE_PENDING_READY_BOOKED = 4
    }


}
