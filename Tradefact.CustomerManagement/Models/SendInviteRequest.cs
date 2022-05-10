using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Models
{
    public class SendInviteRequest
    {
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        [JsonProperty("inviteType")]
        public int InviteType { get; set; }
    }
}
