using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flare.Data.Configuration
{
    public class DeviceReportConfiguration : BaseEntityTypeConfiguration<DeviceReport>
    {
        public override void Configure(EntityTypeBuilder<DeviceReport> builder)
        {
            builder.ToTable("DeviceReports");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DeviceId).IsRequired().HasMaxLength(32).IsUnicode(false).HasColumnOrder(2);
            builder.Property(x => x.ResponseType).IsRequired().HasMaxLength(128).HasColumnOrder(3);
            builder.Property(x => x.ProtocolType).IsRequired().HasMaxLength(32).IsUnicode(false).HasColumnOrder(4);
            builder.Property(x => x.MsgSeqNo).IsRequired().HasColumnOrder(5);

            builder.Property(x => x.GpsLatitude).HasColumnName("GPS_Latitude").HasColumnOrder(11);
            builder.Property(x => x.GpsLongitude).HasColumnName("GPS_Longitude").HasColumnOrder(12);
            builder.Property(x => x.GpsAlarm).HasColumnName("GPS_Alarm").HasMaxLength(64).HasColumnOrder(13);
            builder.Property(x => x.GpsStatus).HasColumnName("GPS_Status").HasMaxLength(250).HasColumnOrder(14);
            builder.Property(x => x.GpsIsPrecise).HasColumnName("GPS_IsPrecise").HasColumnOrder(15);
            builder.Property(x => x.GpsAltitude).HasColumnName("GPS_Altitude").HasColumnOrder(16);
            builder.Property(x => x.GpsSpeed).HasColumnName("GPS_Speed").HasColumnOrder(17);
            builder.Property(x => x.GpsDirection).HasColumnName("GPS_Direction").HasColumnOrder(18);
            builder.Property(x => x.GpsTime).HasColumnName("GPS_Time").HasColumnOrder(19);
            builder.Property(x => x.GpsRecvTime).HasColumnName("GPS_RecvTime").HasColumnOrder(20);
            builder.Property(x => x.GpsUseLbslocation).HasColumnName("GPS_UseLBSLocation").HasColumnOrder(21);
            builder.Property(x => x.GpsAddress).HasColumnName("GPS_Address").HasMaxLength(500).HasColumnOrder(22);

            builder.Property(x => x.Temperature).HasColumnType("decimal(5, 2)").HasColumnOrder(25);
            builder.Property(x => x.Humidity).HasColumnType("decimal(5, 2)").HasColumnOrder(26);
            builder.Property(x => x.Battery).HasColumnOrder(27);

            builder.Property(x => x.CommandId).HasMaxLength(250).HasColumnOrder(30);
            builder.Property(x => x.CommandExecuteResult).HasMaxLength(250).HasColumnOrder(31);
            builder.Property(x => x.ExtraInfo).IsUnicode(false).HasColumnOrder(32);
            builder.Property(x => x.RawData).IsUnicode(false).HasColumnOrder(33);

            base.Configure(builder);
        }
    }
}
