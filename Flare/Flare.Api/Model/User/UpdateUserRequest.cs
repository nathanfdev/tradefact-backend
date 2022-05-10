using Core.Attributes;
using FluentValidation;
using System;

namespace Flare.Api.Model
{
    [TypescriptAutoGeneration]
    public class UpdateUserRequest : UserRequest
    {
        public Guid Id { get; set; }

    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}