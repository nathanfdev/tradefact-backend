using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class ListAddressesRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ListAddressesRequestValidator : AbstractValidator<ListAddressesRequest>
    {
        public ListAddressesRequestValidator() => RuleFor(x => x.CompanyId).NotEmpty();
    }
}