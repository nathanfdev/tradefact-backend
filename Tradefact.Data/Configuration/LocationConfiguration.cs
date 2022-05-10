using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(x => x.LocCode);

            builder.Property(x => x.LocCode).HasMaxLength(8).HasColumnOrder(1);

            builder.Property(x => x.IATA).HasMaxLength(4).HasColumnOrder(2);
            builder.Property(x => x.Name).HasMaxLength(128).HasColumnOrder(3);

            builder.Property(x => x.CountryCode).HasMaxLength(4).HasColumnOrder(4);

            builder.Property(e => e.Port).HasDefaultValue(false).HasColumnOrder(5);
            builder.Property(e => e.Rail).HasDefaultValue(false).HasColumnOrder(6);
            builder.Property(e => e.Airport).HasDefaultValue(false).HasColumnOrder(7);

            builder.Property(x => x.IsGeneric).HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(8);

            builder.OwnsOne(x => x.Position, a =>
            {
                a.Property(p => p.Latitude).HasColumnType("decimal(18, 4)").HasColumnOrder(9);
                a.Property(p => p.Longitude).HasColumnType("decimal(18, 4)").HasColumnOrder(10);
            });
        }
    }

}
