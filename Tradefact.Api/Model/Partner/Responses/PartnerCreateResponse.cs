using Core.Models;

namespace Tradfact.Api.Responses
{
    public class PartnerCreateResponse : CosmosItem<PartnerCreateResponse>
    {
        public string Currency { get; set; }

        public string Name { get; set; }
    }
}