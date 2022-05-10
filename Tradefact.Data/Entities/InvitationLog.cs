using Core.Enums;
using Core.Models;
using System;

namespace Tradefact.Data
{
    public class InvitationLog: BaseEntity<InvitationLog>
    {
        public string UserId { get; set; }
        public string CompanyName { get; set; }
        public string GivenName { get; set; }
        public string EmailAddress { get; set; }
        public int InviteType { get; set; }

        public Guid? MemberOfOrganisationId { get; set; }
        public Guid? InviteRequestedByUserId { get; set; }
        public Guid? InviteRequestedByOrganisationId { get; set; }

        public DateTime? LastActivationAttempt { get; set; }
        public AccountActivationStatus ActivationStatus { get; set; }
        public string SubscriptionPlan { get; set; }

        // public virtual ApplicationUser User { get; set; }
    }

}
