using Core.Models.External;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class ExternalPurchaseOrderLineItemConfiguration : BaseEntityTypeConfiguration<ExternalPurchaseOrderLineItem>
    {

        public override void Configure(EntityTypeBuilder<ExternalPurchaseOrderLineItem> builder)
        {
            builder.ToTable("ExternalPurchaseOrderLineItems", Schemas.Integrations);
            builder.HasKey(x => new { x.ExternalPurchaseOrderId, x.Id });

            builder.Property(x => x.ExternalPurchaseOrderId).HasColumnOrder(1);
            builder.Property(x => x.LineItemID).HasMaxLength(36).HasColumnOrder(1);

            builder.Property(x => x.SKU).HasMaxLength(64).HasColumnOrder(11);
            builder.Property(p => p.Description).HasMaxLength(128).HasColumnOrder(12);
            builder.Property(p => p.SupplierReference).HasMaxLength(128).HasColumnOrder(11);


            builder.Property(p => p.Quantity).HasColumnOrder(12);

            builder.Property(p => p.UnitPrice).HasColumnType("decimal(18, 2)").HasColumnOrder(40);
            builder.Property(p => p.TaxType).HasMaxLength(32).HasColumnOrder(70);
            builder.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(41);
            builder.Property(p => p.LineAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(42);

            base.Configure(builder);
        }
    }


}
