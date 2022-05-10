using Newtonsoft.Json;

namespace FunctionApp.Integration.External.Model.Typeform
{
    public class Choice
    {
        [JsonProperty("label")] public string Label { get; set; }

        [JsonProperty("other")] public string Other { get; set; }
    }
}
