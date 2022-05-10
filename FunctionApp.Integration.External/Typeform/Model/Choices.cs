using Newtonsoft.Json;
using System.Collections.Generic;

namespace FunctionApp.Integration.External.Model.Typeform
{
    public class Choices
    {
        [JsonProperty("labels")] public List<string> Labels { get; set; }

        [JsonProperty("other")] public string Other { get; set; }
    }
}
