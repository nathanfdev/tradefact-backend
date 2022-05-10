using Core.Models;

namespace FunctionApp.Partners.Responses
{
    public class PartnerCreateResponse : CosmosItem<PartnerCreateResponse>
    {
        public string Currency { get; set; }

        public string Name { get; set; }
    }
}