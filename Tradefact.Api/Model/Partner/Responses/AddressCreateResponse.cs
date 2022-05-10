using Core.Models;

namespace Tradfact.Api.Responses
{
    public class AddressCreateResponse : CosmosItem<AddressCreateResponse>
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string County { get; set; }

        public bool IsDefault { get; set; }

        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }
    }
}