using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class DeleteDocumentRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string DocumentId { get; set; }
    }

    public class DeleteDocumentRequestValidator : AbstractValidator<DeleteDocumentRequest>
    {
        public DeleteDocumentRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.DocumentId).NotEmpty();
        }
    }
}