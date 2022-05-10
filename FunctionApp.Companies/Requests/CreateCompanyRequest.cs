
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class CreateCompanyRequest
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
    {
        public CreateCompanyRequestValidator()
        {
            RuleFor(x => x.PartnerId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
        }
    }
}