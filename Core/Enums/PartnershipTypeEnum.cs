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
    public enum PartnershipTypeEnum: int
    {
        [EnumMember(Value = "1")] LOGISTICS = 1,
        [EnumMember(Value = "2")] SUPPLY = 2

        //SUPPLIERAndBUYER = SUPPLIER | BUYER
    }
}
