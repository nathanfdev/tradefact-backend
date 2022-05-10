using Core.Attributes;
using FluentValidation;

namespace Flare.Api.Model
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