using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Core.Models
{
    public class InviteMail : BaseEmail
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("partner")]
        public string Partner { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("organisation_name")]
        public string OrganisationName { get; set; }

        [JsonProperty("invite_type")]
        public int InviteType { get; set; }

        [JsonProperty("already_registered")]
        public bool AlreadyRegistered { get; set; }
    }
}
