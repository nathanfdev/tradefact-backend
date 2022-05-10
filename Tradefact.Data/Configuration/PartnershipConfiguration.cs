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
    public class PartnershipConfiguration : BaseEntityTypeConfiguration<Partnership>
    {
        public override void Configure(EntityTypeBuilder<Partnership> builder)
        {
            builder.ToTable("Partnerships");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderId).HasColumnOrder(2);
            builder.Property(x => x.ClientId).HasColumnOrder(3);

            builder.HasMany(a => a.Shipments).WithOne(t => t.Partnership).HasForeignKey(t => new { t.PartnershipId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.PartnershipTypeId).HasConversion<int>();

            base.Configure(builder);
        }
    }
}
