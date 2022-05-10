using System.Collections.Generic;
using FluentValidation.Results;

namespace Infrastructure.Functions.Helpers
{
    public class ValidatableRequest<T>
    {
        public IList<ValidationFailure> Errors { get; set; }

        public bool IsValid { get; set; }

        public T Value { get; set; }
    }
}