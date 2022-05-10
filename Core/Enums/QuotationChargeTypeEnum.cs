using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Enums
{
    //[Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuotationChargeTypeEnum: int
    {
        [EnumMember(Value = "1")] FREIGHT = 1,
        [EnumMember(Value = "2")] ORIGIN = 2,
        [EnumMember(Value = "3")] DESTINATION = 3,
        [EnumMember(Value = "4")] ADDITIONAL = 4
    }
}
