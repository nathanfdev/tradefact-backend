using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomType
    {
        [EnumMember(Value = "p")] PrivateGroup = 'p',
        [EnumMember(Value = "g")] PrivateGroup2 = 'g',
        [EnumMember(Value = "d")] DirectMessage = 'd',
        [EnumMember(Value = "c")] Channel = 'c'
    }
}