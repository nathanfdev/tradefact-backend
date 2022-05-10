using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tradefact.Data.Configuration
{
    public class PartnershipTypeConfiguration : IEntityTypeConfiguration<PartnershipType>
    {
        public void Configure(EntityTypeBuilder<PartnershipType> builder)
        {
            builder.ToTable("PartnershipTypes");
            builder.HasKey(x => x.PartnershipTypeId);

            builder.Property(x => x.Name).HasColumnOrder(2);
            builder.Property(x => x.ProviderTypeId).HasConversion<int>().HasColumnOrder(3);
            builder.Property(x => x.ClientTypeId).HasConversion<int>().HasColumnOrder(4);

            builder.HasMany(a => a.Partnerships).WithOne(t => t.PartnershipType).HasForeignKey(t => t.PartnershipTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
