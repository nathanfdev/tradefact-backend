using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class EmailRequest
    {
        public string Email { get; set; }
        public bool IsDefault { get; set; }
    }

    public class EmailRequestValidator : AbstractValidator<EmailRequest>
    {
        public EmailRequestValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}