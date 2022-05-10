using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class ListImportsRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ListImportsRequestValidator : AbstractValidator<ListImportsRequest>
    {
        public ListImportsRequestValidator() => RuleFor(x => x.CompanyId).NotEmpty();
    }
}