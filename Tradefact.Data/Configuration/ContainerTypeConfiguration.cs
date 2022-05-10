using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class ContainerTypeConfiguration: IEntityTypeConfiguration<ContainerType>
    {
        public void Configure(EntityTypeBuilder<ContainerType> builder)
        {
            builder.ToTable("ContainerTypes");

            builder.HasKey(x => x.Code);
            builder.Property(x => x.Code).HasMaxLength(4).HasColumnOrder(1);
            builder.Property(x => x.Description).HasMaxLength(128).HasColumnOrder(2);

            builder.Property(x => x.ISOTypeGroup).HasMaxLength(4).HasColumnOrder(3);
            builder.Property(x => x.ISOTypeGroupDescription).HasMaxLength(128).HasColumnOrder(4);

            builder.Property(x => x.Length).HasMaxLength(32).HasColumnOrder(5);
            builder.Property(x => x.Width).HasMaxLength(32).HasColumnOrder(6);
            builder.Property(x => x.Height).HasMaxLength(32).HasColumnOrder(7);

            builder.Property(x => x.AdditionalInformation).HasMaxLength(512).HasColumnOrder(8);

            builder.Property(e => e.Active).HasDefaultValue(false).HasColumnOrder(9);

            builder.Property(e => e.Air).HasDefaultValue(false).HasColumnOrder(10);
            builder.Property(e => e.Sea).HasDefaultValue(false).HasColumnOrder(11);
            builder.Property(e => e.Road).HasDefaultValue(false).HasColumnOrder(12);
        }
    }
}
