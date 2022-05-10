using Core.Models;

namespace FunctionApp.Imports.Models
{
    public class ImportCreateResponse : CosmosItem<ImportCreateResponse>
    {
        public string CompanyId { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Name { get; set; }
    }
}