using FluentValidation;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.ComponentModel.DataAnnotations;

namespace FunctionApp.Search.Requests
{
    [SerializePropertyNamesAsCamelCase]
    public class SearchableCompany
    {
        [IsRetrievable(true)]
        [IsSearchable]
        public SearchableAddress[] Addresses { get; set; }

        [Key]
        [IsRetrievable(true)]
        public string Id { get; set; }

        [IsFilterable]
        [IsRetrievable(true)]
        public string IsActive { get; set; }

        [IsSearchable]
        [IsSortable]
        [IsRetrievable(true)]
        public string Name { get; set; }

        [IsRetrievable(true)]
        public string[] PartnerIds { get; set; }

        [IsRetrievable(true)]
        public int PaymentTerms { get; set; }

        public static string SearchableFields => "name, addresses";

        [IsRetrievable(true)]
        [IsFilterable]
        public string Type { get; set; }

        public class SearchableCompanyValidator : AbstractValidator<SearchableCompany>
        {
            public SearchableCompanyValidator() { }
        }
    }
}