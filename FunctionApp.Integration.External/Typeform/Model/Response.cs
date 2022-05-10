using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

namespace FunctionApp.Integration.External.Model.Typeform
{

    public class Response
    {
        [JsonProperty("event_id")] public string EventId { get; set; }

        [JsonProperty("event_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType EventType { get; set; }

        [JsonProperty("form_response")] public FormResponse FormResponse { get; set; }
    }
}
