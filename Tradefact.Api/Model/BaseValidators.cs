using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model
{
    public class GuidValidator : PropertyValidator
    {
        public GuidValidator() : base("Guid type {PropertyValue} is not a valid type")
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            Guid guidValue = (Guid)context.PropertyValue;
            Guid guid = Guid.NewGuid();

            if (Guid.TryParseExact(guidValue.ToString("B"), "B", out guid))
                return true;

            return false;
        }
    }
}
