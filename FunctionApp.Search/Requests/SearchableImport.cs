using FluentValidation;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.ComponentModel.DataAnnotations;

namespace FunctionApp.Search.Requests
{
    [SerializePropertyNamesAsCamelCase]
    public class SearchableImport
    {
        [IsRetrievable(true)]
        [IsSearchable]
        public SearchableAddress[] Addresses { get; set; }

        [IsRetrievable(true)]
        [IsSearchable]
        public SearchableCompany Company { get; set; }

        [Key]
        [IsRetrievable(true)]
        public string Id { get; set; }

        [IsRetrievable(true)]
        [IsFilterable]
        public bool InvoicePaid { get; set; }

        [IsFilterable]
        [IsRetrievable(true)]
        public string IsActive { get; set; }

        [IsSearchable]
        [IsSortable]
        [IsRetrievable(true)]
        public string Name { get; set; }

        public static string SearchableFields => "name, addresses, company";

        [IsRetrievable(true)]
        [IsFilterable]
        public string Status { get; set; }

        [IsRetrievable(true)]
        [IsFilterable]
        public string Type { get; set; }

        public class SearchableImportValidator : AbstractValidator<SearchableImport>
        {
            public SearchableImportValidator() { }
        }
    }
}