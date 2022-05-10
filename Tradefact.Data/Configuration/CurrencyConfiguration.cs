using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currency");

            builder.HasKey(x => x.CurrencyId);

            builder.Property(x => x.CurrencyId).HasColumnOrder(1);
            builder.Property(x => x.CurrencyCode).HasMaxLength(8).HasColumnOrder(2);
            builder.Property(x => x.CurrencyName).HasMaxLength(128).HasColumnOrder(3);
            builder.Property(x => x.Description).HasMaxLength(128).HasColumnOrder(4);
            builder.Property(x => x.Symbol).HasMaxLength(8).HasColumnOrder(6);

            builder.Property(e => e.Active).HasDefaultValue(false).HasColumnOrder(5);

            builder.HasMany(a => a.Countries).WithOne(t => t.Currency).HasForeignKey(t => t.CurrencyId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
