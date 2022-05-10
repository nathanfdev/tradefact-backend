using Core.Models;

namespace Tradfact.Api.Responses
{
    public class CreateCompanyResponse : CosmosItem<CreateCompanyResponse>
    {
        public string Name { get; set; }
    }
}