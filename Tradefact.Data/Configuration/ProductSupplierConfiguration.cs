using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class ProductSupplierConfiguration : BaseEntityTypeConfiguration<ProductSupplier>
    {
        public override void Configure(EntityTypeBuilder<ProductSupplier> builder)
        {
            builder.ToTable("ProductSuppliers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SupplierReference).HasColumnOrder(20);
            builder.Property(x => x.Price).HasColumnOrder(21);
            builder.Property(x => x.Currency).HasColumnOrder(22);
            builder.Property(x => x.OrderQuantityMinimum).HasColumnOrder(23);

            base.Configure(builder);
        }
    }


}

