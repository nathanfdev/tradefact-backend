using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class ConnectResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}