using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class SupplierConfiguration : BaseEntityTypeConfiguration<Supplier>
    {
        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.ContactName).HasMaxLength(250);
            builder.Property(x => x.ContactEmail).HasMaxLength(250);
            builder.Property(x => x.ContactTelephone).HasMaxLength(50);

            //builder.OwnsOne<Address>(x => x.Address, a =>
            //{
            //    a.ToTable("SupplierAddresses");
            //    a.Property(p => p.AddressLine1).HasMaxLength(250);
            //    a.Property(p => p.AddressLine2).HasMaxLength(250);
            //    a.Property(p => p.AddressLine3).HasMaxLength(250);
            //    a.Property(p => p.AddressLine4).HasMaxLength(250);
            //    a.Property(p => p.PostalCode).HasMaxLength(50);
            //    a.Property(p => p.Province).HasMaxLength(50);
            //    a.Property(p => p.City).HasMaxLength(50);
            //    a.Property(p => p.Country).HasMaxLength(50);
            //});

            // builder.HasMany(a => a.Activities).WithOne(t => t.Supplier).HasForeignKey(t => t.SupplierId).OnDelete(DeleteBehavior.NoAction); ;

            base.Configure(builder);
        }
    }
}
