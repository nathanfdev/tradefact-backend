using Core.Models;

namespace FunctionApp.Suppliers.Responses
{
    public class SupplierCreateResponse : CosmosItem<SupplierCreateResponse>
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string City { get; set; }

        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public string ContactTelephone { get; set; }

        public string Country { get; set; }

        public string County { get; set; }

        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }
    }
}