using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class B2BConnectionConfiguration : BaseEntityTypeConfiguration<B2BConnection>
    {
        public override void Configure(EntityTypeBuilder<B2BConnection> builder)
        {
            builder.ToTable("B2B_Connections");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.OrgansationId).HasName("IX_B2B_Connections_OrganisationID");

            builder.Property(x => x.OrgansationId).HasColumnOrder(2);
            builder.Property(x => x.LinkedOrganisationId).HasColumnOrder(3);

            builder.OwnsMany<ConnectionContact>(x => x.Contacts, a =>
            {
                a.ToTable("B2B_ConnectionContacts");
                a.WithOwner().HasForeignKey(p => p.ConnectionId);
                a.HasKey(x => new { x.ConnectionId, x.Id });
                a.Property(x => x.ConnectionId).HasColumnOrder(2);
                a.Property(x => x.IsActive).HasColumnOrder(4);
                a.Property(x => x.FullName).HasMaxLength(250).HasColumnOrder(31);
                a.Property(x => x.FirstName).HasMaxLength(250).HasColumnOrder(32);
                a.Property(x => x.MiddleName).HasMaxLength(250).HasColumnOrder(33);
                a.Property(x => x.LastName).HasMaxLength(250).HasColumnOrder(34);
                a.Property(x => x.Title).HasMaxLength(250).HasColumnOrder(35);
                a.Property(x => x.Salutation).HasMaxLength(250).HasColumnOrder(36);
                a.Property(x => x.Department).HasMaxLength(250).HasColumnOrder(37);
                a.Property(x => x.Notes).HasMaxLength(250).HasColumnOrder(38);
                a.Property(x => x.IsDefault).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(3);


                a.Property(x => x.Id).HasColumnOrder(1);
                a.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                a.Property(x => x.CreationDate).HasColumnOrder(998);
                a.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                a.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                a.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                a.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                a.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                a.Property(x => x.Timestamp).HasColumnOrder(1005);


                a.OwnsMany<ConnectionContactEmailAddress>(e => e.Email, ce =>
                {
                    ce.ToTable("B2B_ConnectionContactEmails");
                    ce.WithOwner().HasForeignKey(em => new { em.ConnectionId, em.ContactId });
                    ce.HasKey(em => new { em.ContactId, em.Id });
                    ce.Property(cm => cm.Email).HasMaxLength(250);
                    ce.Property(x => x.IsDefault).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever();

                    ce.Property(x => x.Id).HasColumnOrder(1);
                    ce.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                    ce.Property(x => x.CreationDate).HasColumnOrder(998);
                    ce.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                    ce.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                    ce.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                    ce.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                    ce.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                    ce.Property(x => x.Timestamp).HasColumnOrder(1005);
                });

                a.OwnsMany<ConnectionContactPhone>(e => e.Phone, ce =>
                {
                    ce.ToTable("B2B_ConnectionContactPhoneNumbers");
                    ce.WithOwner().HasForeignKey(em => new { em.ConnectionId, em.ContactId });
                    ce.HasKey(em => new { em.ContactId, em.Id });
                    ce.Property(cm => cm.AreaCode).HasMaxLength(8);
                    ce.Property(cm => cm.CountryCode).HasMaxLength(8);
                    ce.Property(cm => cm.Number).HasMaxLength(32);
                    ce.Property(x => x.IsDefault).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever();

                    ce.Property(x => x.Id).HasColumnOrder(1);
                    ce.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                    ce.Property(x => x.CreationDate).HasColumnOrder(998);
                    ce.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                    ce.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                    ce.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                    ce.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                    ce.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                    ce.Property(x => x.Timestamp).HasColumnOrder(1005);

                });

            });


            base.Configure(builder);
        }
    }
}
