using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionTypeEnum
    {
        [EnumMember(Value = "0")] IMPORT = 0,
        [EnumMember(Value = "1")] EXPORT = 1
    }
}
