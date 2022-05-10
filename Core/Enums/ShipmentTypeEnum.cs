using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShipmentTypeEnum
    {
        [EnumMember(Value = "1")] SEA = 1,
        [EnumMember(Value = "2")] AIR = 2,
        [EnumMember(Value = "3")] ROAD = 3,
        [EnumMember(Value = "4")] RAIL = 4
    }

}
