using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class AddUpdateNoteRequest
    {
        public string Note { get; set; }
    }

    public class AddUpdateNoteRequestValidator : AbstractValidator<AddUpdateNoteRequest>
    {
        public AddUpdateNoteRequestValidator()
        {
            // RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Note).NotEmpty().Length(1, 4000);
        }
    }
}