using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Tradefact.Application.PurchaseOrders
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PurchaseOrderViewEnum : int
    {
        [EnumMember(Value = "0")] DEFAULT = 0,
        [EnumMember(Value = "1")] WIDGET = 1,
    }
}
