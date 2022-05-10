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
    public enum OrganisationTypeEnum: int
    {
        [EnumMember(Value = "1")] SHIPPER = 1,
        [EnumMember(Value = "2")] SUPPLIER = 2,
        [EnumMember(Value = "3")] BUYER = 3,
        [EnumMember(Value = "4")] PARTNER = 4,
        [EnumMember(Value = "99")] UNKNOWN = 99
        //SUPPLIERAndBUYER = SUPPLIER | BUYER
    }
}
