using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace FunctionApp.Search.Requests
{
    [SerializePropertyNamesAsCamelCase]
    public class SearchableAddress
    {
        [IsSearchable]
        [IsRetrievable(true)]
        public string AddressLine1 { get; set; }

        [IsSearchable]
        [IsRetrievable(true)]
        public string AddressLine2 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        [IsRetrievable(true)]
        public string City { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        [IsRetrievable(true)]
        public string Country { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        [IsRetrievable(true)]
        public string PostalCode { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        [IsRetrievable(true)]
        public string Province { get; set; }
    }
}
