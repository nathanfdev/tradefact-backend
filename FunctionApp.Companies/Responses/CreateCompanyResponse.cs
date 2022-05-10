using Core.Models;

namespace FunctionApp.Companies.Responses
{
    public class CreateCompanyResponse : CosmosItem<CreateCompanyResponse>
    {
        public string Name { get; set; }
    }
}