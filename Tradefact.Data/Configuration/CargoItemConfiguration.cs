using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class CargoItemConfiguration : IEntityTypeConfiguration<CargoItem>
    {
        public void Configure(EntityTypeBuilder<CargoItem> builder)
        {
            builder.ToTable("CargoItems");

            builder.HasKey(x => new { x.FreightMovementId, x.FreightMovementItemId, x.ItemId });
            builder.HasNoDiscriminator();

            builder.Property(x => x.HsCode).HasMaxLength(32);

            builder.Property(x => x.SKU).HasMaxLength(64);
            builder.Property(x => x.ItemDescription).HasMaxLength(512);
            builder.Property(x => x.UOL).HasMaxLength(16);
            builder.Property(x => x.UOW).HasMaxLength(16);
            builder.Property(x => x.ProductDescriptionOverride);

            builder.HasOne<Product>(x => x.Product)
                .WithMany()     
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ProductVariant>(x => x.ProductVariant)
                .WithMany()
                .HasForeignKey(c => c.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Address>(x => x.PlaceOfLoading)
                .WithMany()
                .HasForeignKey(c => c.PlaceOfLoadingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
