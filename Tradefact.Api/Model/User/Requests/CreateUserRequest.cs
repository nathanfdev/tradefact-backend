
using Core.Attributes;
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    [TypescriptAutoGeneration]
    public class CreateUserRequest: UserRequest
    {
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

}