using Core.Models;
using FluentValidation;
using System;

namespace Tradefact.Api.Model
{
    public class RouteScheduleCriteriaValidator : AbstractValidator<RouteScheduleCriteria>
    {
        public RouteScheduleCriteriaValidator()
        {
            RuleFor(x => x.PortOfLoading).NotEmpty().MinimumLength(3);
            RuleFor(x => x.PortOfDischarge).NotEmpty().MinimumLength(3);

            RuleFor(x => x.EarliestDate)
                .NotEmpty().WithMessage("Required field")
                .GreaterThanOrEqualTo(p => DateTime.Now.ToUniversalTime().Date).WithMessage("Date must not be in the past");
        }
    }

}