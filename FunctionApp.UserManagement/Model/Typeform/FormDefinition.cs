using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tradefact.UserManagement.Model
{
    public class FormDefinition
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("fields")] public List<Field> Fields { get; set; }
    }
}
