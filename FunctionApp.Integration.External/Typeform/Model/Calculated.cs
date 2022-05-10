using Newtonsoft.Json;

namespace FunctionApp.Integration.External.Model.Typeform
{
    public class Calculated
    {
        [JsonProperty("score")] public int Score { get; set; }
    }
}
