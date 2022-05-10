using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class UserProfile
    {
        
        [JsonIgnore]
        public string Id { get; set; }
        
        [JsonIgnore]
        public string Email { get; set; }

        [JsonProperty("displayName")]
        public string FullName { get; set; }
        [JsonProperty("avatar")]
        public string ProfileImage { get; set; }
    }
}
