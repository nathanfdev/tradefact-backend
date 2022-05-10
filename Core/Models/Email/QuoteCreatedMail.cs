using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Core.Models
{
    public class QuoteCreatedMail : BaseEmail
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("organisation_name")]
        public string OrganisationName { get; set; }
    }
}
