using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class DownloadDocumentRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string DocumentId { get; set; }
    }

    public class DownloadDocumentRequestValidator : AbstractValidator<DownloadDocumentRequest>
    {
        public DownloadDocumentRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.DocumentId).NotEmpty();
        }
    }
}