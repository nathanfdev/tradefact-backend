using Core.Models;
using FluentValidation;
using FluentValidation.Validators;
using Newtonsoft.Json;
using System;

namespace Tradfact.Api.Requests
{



    public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
    {
        public UpdateContactRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().Length(3, 150);
        }
    }

    public class UpdateFullContactRequestValidator : AbstractValidator<UpdateFullContactRequest>
    {
        public UpdateFullContactRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().Length(3, 150);
        }
    }



}