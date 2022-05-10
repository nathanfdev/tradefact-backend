using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{
    public class OrganisationRegistrationConfiguration : IEntityTypeConfiguration<OrganisationRegistration>
    {
        public void Configure(EntityTypeBuilder<OrganisationRegistration> builder)
    {
            builder.ToTable("OrganisationRegistrations");
            builder.HasKey(x => new { x.Id });

            builder.Property(x => x.Id).HasColumnOrder(1);
            builder.Property(x => x.CompanyLegalName).HasMaxLength(250).HasColumnOrder(4);
            builder.Property(x => x.Country).HasMaxLength(250).HasColumnOrder(5);
            builder.Property(x => x.Address).HasMaxLength(250).HasColumnOrder(6);
            builder.Property(x => x.City).HasMaxLength(250).HasColumnOrder(7);
            builder.Property(x => x.WebAddress).HasMaxLength(250).HasColumnOrder(8);
            builder.Property(x => x.BusinessId).HasMaxLength(250).HasColumnOrder(9);
            builder.Property(x => x.BusinessRegDocumentName).HasColumnOrder(10);
            builder.Property(x => x.BusinessRegDocumentUrl).HasColumnOrder(11);
            builder.Property(x => x.RegistrationStatus).HasConversion<int>().HasColumnOrder(12);
            builder.Property(x => x.CompanyBio).HasColumnOrder(13);
            builder.Property(x => x.OrganisationId).HasColumnOrder(14);

            builder.OwnsMany<OrganisationRegistrationUser>(x => x.RegistrationUsers, u => {

                u.ToTable("OrganisationRegistrationUsers");
                u.HasKey(x => new { x.OrganisationRegistrationId, x.Id });

                u.WithOwner().HasForeignKey(x => x.OrganisationRegistrationId);

                u.Property(x => x.OrganisationRegistrationId).HasColumnOrder(1);
                u.Property(x => x.Id).HasColumnOrder(2);
                u.Property(x => x.Name).HasMaxLength(250).HasColumnOrder(4);
                u.Property(x => x.Email).HasMaxLength(250).HasColumnOrder(5);
                u.Property(x => x.Role).HasColumnOrder(6);
                u.Property(x => x.IsAdmin).HasColumnOrder(7);

            });

        }
    }
}
