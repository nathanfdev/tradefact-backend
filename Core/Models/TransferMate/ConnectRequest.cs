using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Core.Models.TransferMate
{
    [DataContract]
    public class ConnectRequest
    {
        [DataMember(Name = "password", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [DataMember(Name = "username", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
    }
}
