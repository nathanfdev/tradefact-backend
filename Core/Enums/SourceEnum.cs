using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SourceEnum
    {
        [EnumMember(Value = "1")] Directory = 1,
        [EnumMember(Value = "2")] Tradefact = 2,
        [EnumMember(Value = "3")] Invited = 3
    }
}
