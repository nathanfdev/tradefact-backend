using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class ImportQueryRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string Id { get; set; }
    }

    public class ImportQueryRequestValidator : AbstractValidator<ImportQueryRequest>
    {
        public ImportQueryRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty().Length(3, 150);
            RuleFor(x => x.Id).NotEmpty().Length(3, 150);
        }
    }
}