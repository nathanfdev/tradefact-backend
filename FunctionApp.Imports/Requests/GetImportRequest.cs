using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class GetImportRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }
    }

    public class GetImportRequestValidator : AbstractValidator<GetImportRequest>
    {
        public GetImportRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}