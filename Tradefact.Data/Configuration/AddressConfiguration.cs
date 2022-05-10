using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class AddressConfiguration : BaseEntityTypeConfiguration<Address>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrganisationId).HasColumnOrder(2);
            builder.Property(x => x.Type).HasColumnOrder(3);
            builder.Property(x => x.IsDefault).HasColumnOrder(4);
            builder.Property(x => x.IsInvoiceAddress).HasColumnOrder(5);

            builder.Property(x => x.Name).HasMaxLength(250).HasColumnOrder(20);
            builder.Property(x => x.AddressLine1).HasMaxLength(250).HasColumnOrder(21);
            builder.Property(x => x.AddressLine2).HasMaxLength(250).HasColumnOrder(22);
            builder.Property(x => x.AddressLine3).HasMaxLength(250).HasColumnOrder(23);
            builder.Property(x => x.AddressLine4).HasMaxLength(250).HasColumnOrder(24);
            builder.Property(x => x.City).HasMaxLength(250).HasColumnOrder(25);
            builder.Property(x => x.County).HasMaxLength(250).HasColumnOrder(26);
            builder.Property(x => x.Province).HasMaxLength(250).HasColumnOrder(27);
            builder.Property(x => x.PostalCode).HasMaxLength(50).HasColumnOrder(28);

            builder.HasOne<Country>(x => x.Country) 
                .WithMany()       // <---
                .HasForeignKey(c => c.CountryCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Position, a =>
            {
                a.Property(p => p.Latitude).HasColumnType("decimal(18, 4)").HasColumnOrder(9);
                a.Property(p => p.Longitude).HasColumnType("decimal(18, 4)").HasColumnOrder(10);
            });

            //builder.HasIndex(x => new { x.PartnerId, x.IsActive }).HasName("IX_AddressPartnerIsActive"); 
            //builder.HasIndex(x => new { x.CompanyId, x.IsActive }).HasName("IX_AddressCompanyIsActive");

            base.Configure(builder);

        }
    }
}
