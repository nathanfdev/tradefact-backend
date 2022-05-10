using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class PartnerRateConfiguration : CosmoItemTypeConfiguration<PartnerRate>
    {
        public override void Configure(EntityTypeBuilder<PartnerRate> builder)
        {
            builder.ToTable("PartnerRates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Operator).HasMaxLength(12);
            builder.Property(x => x.PortCode).HasMaxLength(12);
            builder.Property(x => x.PortOfLoading).HasMaxLength(12);
            builder.Property(x => x.PortOfDischarge).HasMaxLength(12);
            builder.Property(x => x.Country).HasMaxLength(250);
            builder.Property(x => x.Customs).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.Documents).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.ExportClearanceUsd).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.ExportThcCost).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.ExportThcUsd).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.FT20).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.FT40).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.FTHHQ40).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.FTHQ45).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.InboundThc).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.Insurance).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.PortFees).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.RoadCollection).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.RoadDelivery).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.Supplier).HasMaxLength(250);

            base.Configure(builder);
        }
    }
}
