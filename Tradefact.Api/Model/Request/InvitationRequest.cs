using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Request
{
    public class InvitationRequest
    {
        public string CompanyName { get; set; }
        public string GivenName { get; set; }
        public string EmailAddress { get; set; }
        public int InviteType { get; set; }
    }

    public class InvitationRequestValidator : AbstractValidator<InvitationRequest>
    {
        public InvitationRequestValidator()
        {
            RuleFor(x => x.EmailAddress).EmailAddress();
            RuleFor(x => x.InviteType).InclusiveBetween(1,7);
        }
    }
}
