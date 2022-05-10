using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flare.Data.Configuration
{
    class OrderLineItemConfiguration : BaseEntityTypeConfiguration<OrderLineItem>
    {
        public override void Configure(EntityTypeBuilder<OrderLineItem> builder)
        {
            builder.ToTable("OrderLineItems");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId).HasColumnOrder(2);
            builder.Property(x => x.LineNumber).HasColumnOrder(3);
            builder.Property(x => x.LabelText).IsUnicode().HasMaxLength(250).HasColumnOrder(4);
            builder.Property(x => x.DeviceId).HasColumnOrder(5);

            base.Configure(builder);
        }
    }
}
