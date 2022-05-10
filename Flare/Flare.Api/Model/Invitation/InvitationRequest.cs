using FluentValidation;

namespace Flare.Api.Model
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
