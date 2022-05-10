using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class PartnerRateHaulageConfiguration : BaseEntityTypeConfiguration<PartnerRateHaulage>
    {
        public override void Configure(EntityTypeBuilder<PartnerRateHaulage> builder)
        {
            builder.ToTable("PartnerHaulageRates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.City).HasMaxLength(50);
            builder.Property(x => x.Country).HasMaxLength(50);
            builder.Property(x => x.Gbbfs20Ft).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.Gbbfs40Ft).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.Gfbbs40Ft).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.City).HasMaxLength(50);
            builder.Property(x => x.Country).HasMaxLength(50);

            base.Configure(builder);
        }
    }
}
