using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuotationStateEnum
    {
        [EnumMember(Value = "0")] PENDING = 0,
        [EnumMember(Value = "1")] READY = 1,
        [EnumMember(Value = "2")] ACCEPTED = 2,
        [EnumMember(Value = "3")] EXPIRED = 3,
        [EnumMember(Value = "4")] REJECTED = 4
    }
}
