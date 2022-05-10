
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
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