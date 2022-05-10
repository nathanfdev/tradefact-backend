using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.AADB2C.Api.Model.Request
{
    public class InvitationRequest
    {
        public string CompanyName { get; set; }
        public string EmailAddress { get; set; }
        public string PersonName { get; set; }
        public int InviteType { get; set; }
        public string InviteByName { get; set; }
    }

    public class InvitationRequestValidator : AbstractValidator<InvitationRequest>
    {
        public InvitationRequestValidator()
        {
            RuleFor(x => x.EmailAddress).EmailAddress();
            RuleFor(x => x.CompanyName).NotEmpty().Length(3, 150);
            RuleFor(x => x.PersonName).NotEmpty().Length(3, 150);

            RuleFor(x => x.InviteType).InclusiveBetween(0,4);
        }
    }
}
