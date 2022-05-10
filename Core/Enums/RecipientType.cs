using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecipientType
    {
        [EnumMember(Value = "1")] TradefactForwarder = 1,
        [EnumMember(Value = "2")] EmailForwarder = 2,
        [EnumMember(Value = "3")] Other = 3
    }
}
