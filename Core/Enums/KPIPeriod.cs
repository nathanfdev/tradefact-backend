using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KPIPeriod
    {
        Today = 1,
        ThisWeek = 2,
        ThisMonth = 3,
        ThisYear = 4,
        AllTime = 5
    }
}
