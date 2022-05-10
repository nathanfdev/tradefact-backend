using Core.Enums;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class ListDocumentsRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }

        public bool IsActive { get; set; } = true;

        public OwnershipEnum Ownership { get; set; }
    }

    public class ListDocumentsRequestValidator : AbstractValidator<ListDocumentsRequest>
    {
        public ListDocumentsRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}