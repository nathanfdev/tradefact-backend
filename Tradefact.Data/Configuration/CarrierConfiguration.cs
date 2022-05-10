using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{

    public class CarrierConfiguration: IEntityTypeConfiguration<Carrier>
    {
        public void Configure(EntityTypeBuilder<Carrier> builder)
        {
            builder.ToTable("Carriers");

            builder.HasKey(x => x.SCAC);
            builder.Property(x => x.SCAC).HasMaxLength(4);

            builder.Property(x => x.Name).HasMaxLength(128);
            builder.Property(x => x.Description).HasMaxLength(128);
            builder.Property(x => x.CarrierGroup).HasMaxLength(128);
            builder.Property(x => x.Company).HasMaxLength(128);
            builder.Property(x => x.Regions).HasMaxLength(128);
            builder.Property(x => x.Active).HasDefaultValue(true).ValueGeneratedNever();

            builder.OwnsOne(x => x.Tracking, a =>
            {
                a.Property(x => x.Enabled).HasDefaultValue(false);
                a.Property(p => p.OperatorValue).HasMaxLength(16);
                a.Property(x => x.BillOfLading).HasDefaultValue(false);
                a.Property(x => x.Container).HasDefaultValue(false);
            });

            builder.Property(e => e.ShipmentTypeId).HasConversion<int>();
        }
    }
}
