using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class DeactivateImportRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }
    }

    public class DeactivateImportRequestValidator : AbstractValidator<DeactivateImportRequest>
    {
        public DeactivateImportRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}