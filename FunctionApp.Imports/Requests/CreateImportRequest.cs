using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class CreateImportRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string PartnerId { get; set; }
    }

    public class CreateImportRequestValidator : AbstractValidator<CreateImportRequest>
    {
        public CreateImportRequestValidator()
        {
            RuleFor(x => x.PartnerId).NotEmpty();
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
        }
    }
}