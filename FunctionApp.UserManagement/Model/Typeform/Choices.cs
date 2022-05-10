using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tradefact.UserManagement.Model
{
    public class Choices
    {
        [JsonProperty("labels")] public List<string> Labels { get; set; }

        [JsonProperty("other")] public string Other { get; set; }
    }
}
