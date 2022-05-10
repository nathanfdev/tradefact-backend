using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{
    [Obsolete]
    public class ImportConfiguration : CosmoItemTypeConfiguration<Import>
    {
        public override void Configure(EntityTypeBuilder<Import> builder)
        {
            builder.ToTable("Imports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CollectedDate).IsRequired(false).HasColumnType("datetime2");
            builder.Property(x => x.CompanyId).HasMaxLength(50);
            builder.Property(x => x.ContainerNumber).HasMaxLength(50);
            builder.Property(x => x.CustomsStatus).HasMaxLength(50);
            builder.Property(x => x.DeliveredDate).IsRequired(false).HasColumnType("datetime2");
            builder.Property(x => x.ETA).IsRequired(false).HasColumnType("datetime2");
            builder.Property(x => x.Latitude).HasMaxLength(50);
            builder.Property(x => x.Longitude).HasMaxLength(50);
            builder.Property(x => x.Name).HasMaxLength(50);
            builder.Property(x => x.PartnerId).HasMaxLength(50);
            builder.Property(x => x.PoNumber).HasMaxLength(50);
            builder.Property(x => x.ShippingProviderId).HasMaxLength(50);
            // builder.Property(x => x.SupplierId).HasMaxLength(50);
            builder.Property(x => x.VesselName).HasMaxLength(50);
            builder.Property(x => x.InvoicePaid);

            builder.Ignore(x => x.PurchaseOrders);
            builder.Ignore(x => x.ConsignmentDetails);


            // builder.HasMany(a => a.Quotations).WithOne(t => t.Import).HasForeignKey(t => t.ImportId);


            base.Configure(builder);
        }
    }
}
