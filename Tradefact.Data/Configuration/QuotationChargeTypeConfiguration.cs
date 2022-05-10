using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class QuotationChargeTypeConfiguration : IEntityTypeConfiguration<QuotationChargeType>
    {
        public void Configure(EntityTypeBuilder<QuotationChargeType> builder)
        {
            builder.ToTable("QuotationChargeType");

            builder.HasKey(x => x.QuotationChargeTypeId);
            builder.Property(x => x.QuotationChargeTypeId).HasConversion<int>();
            builder.Property(x => x.Name).HasMaxLength(32);
        }
    }

}
