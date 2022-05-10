
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class ListCompaniesRequest
    {
        public bool IsActive { get; set; } = true;

        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class ListCompaniesRequestValidator : AbstractValidator<ListCompaniesRequest>
    {
        public ListCompaniesRequestValidator() => RuleFor(x => x.PartnerId).NotEmpty();
    }
}