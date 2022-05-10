using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class CountryConfiguration: IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            builder.HasKey(x => x.Code2);
            builder.Property(x => x.Code2).HasMaxLength(4).HasColumnOrder(1);

            builder.Property(x => x.Code3).HasMaxLength(8).HasColumnOrder(2);
            builder.Property(x => x.Name).HasMaxLength(256).HasColumnOrder(3);

            builder.HasMany(a => a.Locations).WithOne(t => t.Country).HasForeignKey(t => t.CountryCode).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Currencies).WithOne(t => t.Country).HasForeignKey(t => t.CountryCode).OnDelete(DeleteBehavior.Restrict);
        }
    }

}
