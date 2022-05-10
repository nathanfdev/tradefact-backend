using Newtonsoft.Json;

namespace Tradefact.Portal.Models
{
    public class OrganisationSearchModel
    {
        [JsonProperty("organisationId")]
        public string OrganisationId;

        [JsonProperty("organisationName")]
        public string OrganisationName;
    }
}
