
using FluentValidation;

namespace FunctionApp.Search.Requests
{
    public class CompanySearchRequest
    {
        public string Filter { get; set; }

        public string OrderBy { get; set; } = "UpsertDate";

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SearchTerm { get; set; } = "*";
    }

    public class CompanySearchRequestValidator : AbstractValidator<CompanySearchRequest>
    {
        //public CompanySearchRequestValidator() => RuleFor(x => x.SearchTerm).NotEmpty().Length(3, 10);
    }
}