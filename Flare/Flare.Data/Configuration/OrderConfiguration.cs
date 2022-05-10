using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flare.Data.Configuration
{
    public class OrderConfiguration : BaseEntityTypeConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrganisationId).HasColumnOrder(2);
            builder.Property(x => x.ShipmentName).HasMaxLength(100).HasColumnOrder(3);
            builder.Property(x => x.ReferenceType).HasMaxLength(100).HasColumnOrder(4);
            builder.Property(x => x.ReferenceNumber).HasMaxLength(100).HasColumnOrder(5);
            builder.Property(x => x.TransportMode).HasColumnOrder(6);
            builder.Property(x => x.ShipmentDate).HasColumnOrder(7);
            builder.Property(x => x.DeliveryAddressId).HasColumnOrder(8);
            builder.Property(x => x.DestinationAddressId).HasColumnOrder(9);
            builder.Property(x => x.OrderStatus).HasColumnOrder(10);

            builder.HasOne<Address>(x => x.DeliveryAddress).WithMany().HasForeignKey(x => x.DeliveryAddressId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Address>(x => x.DestinationAddress).WithMany().HasForeignKey(x => x.DestinationAddressId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.LineItems).WithOne().HasForeignKey(y => new { y.OrderId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }
}
