using Core.Models;

namespace FunctionApp.Companies.Responses
{
    public class CreateProductResponse : CosmosItem<CreateProductResponse>
    {
        public string CompanyId { get; set; }

        public string Description { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }
    }
}