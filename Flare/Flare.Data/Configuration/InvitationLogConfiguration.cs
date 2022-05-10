using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flare.Data.Configuration
{
    public class InvitationLogConfiguration : BaseEntityTypeConfiguration<InvitationLog>
    {
        public override void Configure(EntityTypeBuilder<InvitationLog> builder)
        {
            builder.ToTable("AspNetUserInvitations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyName).HasColumnOrder(250);
            builder.Property(x => x.EmailAddress).HasColumnOrder(251);
            builder.Property(x => x.GivenName).HasColumnOrder(252);

            builder.Property(x => x.InviteRequestedByUserId).HasColumnOrder(253);
            builder.Property(x => x.InviteRequestedByOrganisationId).HasColumnOrder(254);
            builder.Property(x => x.MemberOfOrganisationId).HasColumnOrder(255);
            builder.Property(x => x.LastActivationAttempt).HasColumnOrder(256);
            builder.Property(x => x.ActivationStatus).HasColumnOrder(257);
            builder.Property(x => x.SubscriptionPlan).HasColumnOrder(258);

            base.Configure(builder);
        }
    }
}
