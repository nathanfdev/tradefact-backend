using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class CurrencyCountryConfiguration : IEntityTypeConfiguration<CurrencyCountry>
    {
        public void Configure(EntityTypeBuilder<CurrencyCountry> builder)
        {
            builder.ToTable("CurrencyCountries");

            builder.HasKey(x => new { x.CurrencyId, x.CountryCode });

            builder.Property(x => x.CurrencyId).HasColumnOrder(1);
            builder.Property(x => x.CountryCode).HasMaxLength(4).HasColumnOrder(2);

            builder.Property(e => e.Active).HasDefaultValue(false).HasColumnOrder(5);
        }
    }

}
