using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class AirlineConfiguration : IEntityTypeConfiguration<Airline>
    {
        public void Configure(EntityTypeBuilder<Airline> builder)
        {
            builder.ToTable("Airlines");

            builder.HasKey(x => new { x.AWBPrefix, x.IATA2LetterCcode });
            builder.Property(x => x.AWBPrefix).HasMaxLength(3);
            builder.Property(x => x.IATA2LetterCcode).HasMaxLength(2);

            builder.Property(x => x.Name).HasMaxLength(256);
            builder.Property(x => x.Active).HasDefaultValue(true).ValueGeneratedNever();

            builder.OwnsOne(x => x.Tracking, a =>
            {
                a.Property(x => x.Enabled).HasDefaultValue(false);
                a.Property(p => p.Provider).HasMaxLength(32);
            });
        }
    }
}
