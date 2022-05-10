using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{

    public class FreightMovementConfiguration : BaseEntityTypeConfiguration<FreightMovement>
    {
        public override void Configure(EntityTypeBuilder<FreightMovement> builder)
        {
            builder.HasKey(x => new { x.Id });

            builder.ToTable("FreightMovements");

            builder.Property(x => x.ShipmentType).HasColumnOrder(11);
            builder.Property(x => x.IncoTerms).HasColumnOrder(12);
            builder.Property(x => x.LoadType).HasColumnOrder(13);
            builder.Property(x => x.TransactionType).HasColumnOrder(14);
            builder.Property(x => x.GoodsReady).HasColumnOrder(15);

            builder.Property(x => x.Name).HasMaxLength(64).HasColumnOrder(20);
            builder.Property(x => x.Reference).HasMaxLength(this.MaxReferenceLength).HasColumnOrder(21);

            builder.Property(x => x.ConsignmentQuantity).HasColumnOrder(25);

            builder.Property(x => x.PlaceOfLoadingId).HasColumnOrder(30);
            builder.Property(x => x.PortOfLoadingId).HasMaxLength(8).HasColumnOrder(31);
            builder.Property(x => x.PortOfDischargeId).HasMaxLength(8).HasColumnOrder(32);
            builder.Property(x => x.PlaceOfDispatchId).HasColumnOrder(33);

            builder.Property(x => x.CustomsBrokerageRequired).HasColumnOrder(40);

            builder.Property(x => x.InsuranceRequired).HasColumnOrder(50);
            builder.Property(x => x.InsuranceCurrency).HasMaxLength(8).HasColumnOrder(51);
            builder.Property(x => x.InsuranceValue).HasColumnOrder(52);

            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(55);

            builder.Property(x => x.SupplierId).HasColumnOrder(60);
            builder.Property(x => x.BuyerId).HasColumnOrder(61);


            builder.Property(x => x.NumberOfItems).HasColumnOrder(70);
            builder.Property(x => x.Hazard).HasDefaultValue(false).HasColumnOrder(71);

            builder.Property(x => x.Tags).HasMaxLength(512).HasColumnOrder(100);
            builder.Property(x => x.HSCodes).HasColumnOrder(101);
            builder.Property(x => x.Notes).HasColumnOrder(102);

            builder.Property(x => x.PlaceOfLoadingMultiple).HasMaxLength(256).HasColumnOrder(110);

            builder.Ignore(x => x.FCL);
            builder.Ignore(x => x.LCL);


            builder.HasOne<Address>(x => x.PlaceOfLoading)
                .WithMany()       // <---
                .HasForeignKey(c => c.PlaceOfLoadingId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Address>(x => x.PlaceOfDispatch)
                .WithMany()       // <---
                .HasForeignKey(c => c.PlaceOfDispatchId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Location>(x => x.PortOfLoading)
                .WithMany()       // <---
                .HasForeignKey(c => c.PortOfLoadingId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Location>(x => x.PortOfDischarge)
                .WithMany()       // <---
                .HasForeignKey(c => c.PortOfDischargeId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Items).WithOne(t => t.FreightMovement).HasForeignKey(t => new { t.FreightMovementId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.QuotationRequests).WithOne(t => t.FreightMovement).HasForeignKey(t => new { t.FreightMovementId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Quotations).WithOne(t => t.FreightMovement).HasForeignKey(t => new { t.FreightMovementId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Shipments).WithOne(t => t.FreightMovement).HasForeignKey(t => new { t.FreightMovementId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }

}
