using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flare.Data.Configuration
{
    class DeviceConfiguration : BaseEntityTypeConfiguration<Device>
    {
        public class InvitationLogConfiguration : BaseEntityTypeConfiguration<Device>
        {
            public override void Configure(EntityTypeBuilder<Device> builder)
            {
                builder.ToTable("Devices");
                builder.HasKey(x => x.Id);

                builder.Property(x => x.DeviceId).IsRequired().HasMaxLength(32).HasColumnOrder(2);
                builder.Property(x => x.DeviceType).IsRequired().HasMaxLength(32).HasColumnOrder(3);
                builder.Property(x => x.SerialNumber).HasMaxLength(32).HasColumnOrder(4);
                builder.Property(x => x.IMEI).HasMaxLength(16).HasColumnOrder(5);
                builder.Property(x => x.SimProvider).HasMaxLength(32).HasColumnOrder(6);
                builder.Property(x => x.IMSI).HasMaxLength(15).HasColumnOrder(7);
                builder.Property(x => x.MSISDN).HasMaxLength(15).HasColumnOrder(8);
                builder.Property(x => x.HasFault).IsRequired().HasColumnOrder(9);

                builder.HasOne<DeviceReport>(x => x.LastDeviceReport)
                    .WithMany()
                    .HasForeignKey(x => x.LastDeviceReportId)
                    .OnDelete(DeleteBehavior.Restrict);

                base.Configure(builder);
            }
        }
    }
}
