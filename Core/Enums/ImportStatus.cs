using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImportStatus
    {
        Created = 1,
        PreBooking = 2,
        Booked = 3,
        Collected = 4,
        Loaded = 5,
        Shipping = 6,
        Customs = 7,
        RoadHaulage = 8,
        Delivered = 9,
        Rejected = 10
    }
}
