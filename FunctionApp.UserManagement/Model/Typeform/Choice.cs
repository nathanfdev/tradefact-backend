using Newtonsoft.Json;

namespace Tradefact.UserManagement.Model
{
    public class Choice
    {
        [JsonProperty("label")] public string Label { get; set; }

        [JsonProperty("other")] public string Other { get; set; }
    }
}
