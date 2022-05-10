using Newtonsoft.Json;

namespace Tradefact.UserManagement.Model
{
    public class Calculated
    {
        [JsonProperty("score")] public int Score { get; set; }
    }
}
