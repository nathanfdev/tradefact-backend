using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{

    public class PurchaseOrderConfiguration : BaseEntityTypeConfiguration<PurchaseOrder>
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(x => new { x.Id });

            builder.ToTable("PurchaseOrders");

            builder.Property(x => x.CompanyId).HasColumnOrder(5);
            builder.Property(x => x.PurchaseOrderNumber).HasMaxLength(64).HasColumnOrder(6);
            builder.Property(x => x.SupplierId).HasColumnOrder(7);

            builder.Property(x => x.PurchaseOrderDate).HasColumnOrder(11);
            builder.Property(x => x.GoodsReadyDate).HasColumnOrder(12);
            builder.Property(x => x.TargetDeliveryDate).HasColumnOrder(13);

            builder.Property(x => x.Reference).HasMaxLength(this.MaxReferenceLength).HasColumnOrder(14);
            builder.Property(x => x.Status).HasColumnOrder(15);
            builder.Property(x => x.Stage).HasColumnOrder(16);

            builder.Property(x => x.ShipmentType).HasColumnOrder(20);
            builder.Property(x => x.IncoTerms).HasColumnOrder(21);
            builder.Property(x => x.LoadType).HasColumnOrder(22);
            builder.Property(x => x.TransactionType).HasColumnOrder(23);
            builder.Property(x => x.IncoTerms).HasMaxLength(8).HasColumnOrder(24);

            builder.Property(x => x.PlaceOfLoadingId).HasColumnOrder(30);
            builder.Property(x => x.PortOfLoadingId).HasMaxLength(8).HasColumnOrder(31);
            builder.Property(x => x.PortOfDischargeId).HasMaxLength(8).HasColumnOrder(32);
            builder.Property(x => x.PlaceOfDispatchId).HasColumnOrder(33);

            builder.Property(x => x.Language).HasMaxLength(4).HasColumnOrder(40);
            builder.Property(x => x.PurchasingGroup).HasMaxLength(8).HasColumnOrder(41);

            builder.Property(x => x.CorrespncExternalReference).HasMaxLength(this.MaxReferenceLength).HasColumnOrder(42);
            builder.Property(x => x.CorrespncInternalReference).HasMaxLength(this.MaxReferenceLength).HasColumnOrder(43);

            builder.Property(x => x.RejectedReason).HasColumnOrder(60);
            builder.Property(x => x.OrderRejected).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(70);


            builder.Property(x => x.Submitted).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(80);
            builder.Property(x => x.SubmittedDate).HasColumnOrder(81);

            builder.Property(x => x.Accepted).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(82);
            builder.Property(x => x.AcceptedDate).HasColumnOrder(83);

            builder.Property(x => x.Rejected).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(84);
            builder.Property(x => x.RejectedDate).HasColumnOrder(85);

            builder.Property(x => x.InProduction).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(86);
            builder.Property(x => x.InProductionDate).HasColumnOrder(87);

            builder.Property(x => x.PreShipment).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(88);
            builder.Property(x => x.PreShipmentDate).HasColumnOrder(89);

            builder.Property(x => x.Shipping).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(90);
            builder.Property(x => x.ShippedDate).HasColumnOrder(91);

            builder.Property(x => x.Cancelled).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(92);
            builder.Property(x => x.CancelledDate).HasColumnOrder(93);

            builder.Property(x => x.Completed).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(94);
            builder.Property(x => x.CompletedDate).HasColumnOrder(95);

            builder.Property(x => x.Deleted).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(96);
            builder.Property(x => x.DeletionDate).HasColumnOrder(97);

            builder.Property(x => x.CurrencyId).HasMaxLength(16).HasColumnOrder(100);
            builder.Property(x => x.ExchangeRate).HasColumnOrder(101);
            builder.Property(x => x.InverseExchangeRate).HasColumnOrder(102);

            builder.Property(x => x.CustomsBrokerageRequired).HasColumnOrder(110);

            builder.Property(x => x.InsuranceRequired).HasColumnOrder(120);
            builder.Property(x => x.InsuranceCurrency).HasMaxLength(8).HasColumnOrder(121);
            builder.Property(x => x.InsuranceValue).HasColumnOrder(122);

            builder.Property(x => x.HSCodes).HasMaxLength(512).HasColumnOrder(101);

            builder.OwnsOne<Total>(x => x.ItemsTotal, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(180);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(181);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(182);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(183);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(184);
            });

            builder.OwnsOne<Total>(x => x.ChargesTotal, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(190);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(191);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(192);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(193);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(194);
            });


            builder.OwnsOne<Total>(x => x.Total, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(200);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(201);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(202);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(203);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(204);
            });

            builder.OwnsOne<Total>(x => x.BaseCurrency, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(250);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(251);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(252);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(253);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(254);
            });

            builder.Property(p => p.TaxRate).HasColumnOrder(260);
            builder.Property(p => p.TaxRateDescription).HasMaxLength(256).HasColumnOrder(261);

            builder.Property(x => x.NumberOfItems).HasColumnOrder(290);

            builder.Property(x => x.PaymentTerms).HasColumnType("nvarchar(max)").HasColumnOrder(300);

            builder.Property(x => x.Tags).HasMaxLength(512).HasColumnOrder(350);
            builder.Property(x => x.Containers).HasMaxLength(512).HasColumnOrder(351);

            builder.Property(x => x.Signature).HasColumnOrder(360);
            builder.Property(x => x.AdditionalInformation).HasColumnOrder(361);
            builder.Property(x => x.LogisticsNotes).HasColumnOrder(362);
            builder.Property(x => x.AdditionalSupplierInformation).HasColumnOrder(363);

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


            builder.HasMany(a => a.PurchaseOrderNotes).WithOne(p=>p.PurchaseOrder).HasForeignKey(t => t.PurchaseOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.PurchaseOrderItems).WithOne(t => t.PurchaseOrder).HasForeignKey(t => t.PurchaseOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.AdditionalCharges).WithOne(p => p.PurchaseOrder).HasForeignKey(c => new { c.PurchaseOrderId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(x => x.TagsList);

            builder.Property(x => x.IsLocked).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever();

            base.Configure(builder);
        }
    }

    public class PurchaseOrderItemConfiguration : BaseEntityTypeConfiguration<PurchaseOrderItem>
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
        {
            builder.HasKey(x => new { x.PurchaseOrderId, x.Id });

            builder.ToTable("PurchaseOrderItems");

            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(0);

            builder.Property(x => x.ProductId).HasColumnOrder(20);
            builder.Property(x => x.SKU).HasMaxLength(64).HasColumnOrder(23);

            builder.Property(x => x.PurchaseOrderItemText).HasMaxLength(256).HasColumnOrder(30);

            builder.Property(x => x.OrderQuantity).HasColumnOrder(40);
            builder.Property(x => x.OrderQuantityUnit).HasMaxLength(32).HasColumnOrder(41);
            builder.Property(p => p.NetPriceAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(43);
            builder.Property(p => p.OrderPriceUnit).HasColumnOrder(42);

            builder.Property(p => p.NetPriceQuantity).HasColumnOrder(44);
            builder.Property(p => p.SupplierReference).HasColumnOrder(47);

            builder.Property(x => x.TaxCode).HasMaxLength(8).HasColumnOrder(50);
            builder.Property(x => x.TaxCountry).HasMaxLength(8).HasColumnOrder(51);
            builder.Property(x => x.TaxJurisdiction).HasColumnOrder(52);
            builder.Property(x => x.TaxDeterminationDate).HasColumnOrder(53);

            builder.Property(x => x.IsDeliveryComplete).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(60);
            builder.Property(x => x.IsFinallyInvoiced).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(61);

            builder.Property(x => x.PurchaseOrderItemCategory).HasMaxLength(8).HasColumnOrder(70);
            builder.Property(x => x.AccountAssignmentCategory).HasMaxLength(8).HasColumnOrder(71);
            builder.Property(x => x.PurchaseContract).HasMaxLength(64).HasColumnOrder(72);
        
            builder.Property(p => p.ItemNetWeight).HasColumnType("decimal(18, 2)").HasColumnOrder(80);
            builder.Property(x => x.ItemWeightUnit).HasMaxLength(8).HasColumnOrder(81);

            builder.Property(p => p.ItemVolume).HasColumnType("decimal(18, 2)").HasColumnOrder(90);
            builder.Property(x => x.ItemVolumeUnit).HasMaxLength(8).HasColumnOrder(91);


            //builder.HasOne<Product>(x => x.Product)
            //    .WithMany()
            //    .HasForeignKey(c => c.ProductId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne<ProductVariant>(x => x.ProductVariant)
            //    .WithMany()
            //    .HasForeignKey(c => c.ProductVariantId)
            //    .OnDelete(DeleteBehavior.Restrict);



            builder.HasMany(a => a.PurchaseOrderItemNotes).WithOne(t => t.PurchaseOrderItem).HasForeignKey(t => new { t.PurchaseOrderId, t.Id }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.ScheduleLines).WithOne(t => t.PurchaseOrderItem).HasForeignKey(t => new { t.PurchaseOrderId, t.PurchaseOrderItemId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }

    public class PurchaseOrderNoteConfiguration : BaseEntityTypeConfiguration<PurchaseOrderNote>
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrderNote> builder)
        {
            builder.HasKey(x => new { x.PurchaseOrderId, x.Id });

            builder.ToTable("PurchaseOrderNotes");

            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(0);

            builder.Property(x => x.TextObjectType).HasColumnOrder(30);
            builder.Property(x => x.Language).HasMaxLength(4).HasColumnOrder(31);
            builder.Property(x => x.PlainLongText).HasColumnOrder(32);

            base.Configure(builder);
        }
    }

    public class PurchaseOrderItemNoteConfiguration : BaseEntityTypeConfiguration<PurchaseOrderItemNote>
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrderItemNote> builder)
        {
            builder.HasKey(x => new { x.PurchaseOrderId, x.PurchaseOrderItemId , x.Id });

            builder.ToTable("PurchaseOrderItemNotes");

            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(0);
            builder.Property(x => x.PurchaseOrderItemId).HasColumnOrder(1);

            builder.Property(x => x.TextObjectType).HasColumnOrder(30);
            builder.Property(x => x.Language).HasMaxLength(4).HasColumnOrder(31);
            builder.Property(x => x.PlainLongText).HasColumnOrder(32);

            base.Configure(builder);
        }
    }

    public class PurchaseOrderChargeItemConfiguration : IEntityTypeConfiguration<PurchaseOrderChargeItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderChargeItem> builder)
        {
            builder.ToTable("PurchaseOrderAdditionalCharges");

            builder.HasKey(x => new { x.PurchaseOrderId, x.Id });
            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(0);

            builder.Property(p => p.Type).HasColumnOrder(4);
            builder.Property(p => p.Description).HasMaxLength(512).HasColumnOrder(5);

            builder.Property(p => p.Quantity).HasColumnOrder(200);
            builder.Property(p => p.Rate).HasColumnOrder(201);


            builder.OwnsOne<Total>(x => x.Total, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(250);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(251);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(252);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(253);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(254);
            });

            builder.OwnsOne<Total>(x => x.BaseCurrencyTotal, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(200);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(201);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(202);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(203);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(204);
            });
        }
    }

    public class PurchaseOrderDocumentConfiguration : IEntityTypeConfiguration<PurchaseOrderDocument>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderDocument> builder)
        {
            builder.ToTable("PurchaseOrderDocuments");

            builder.HasKey(x => new { x.PurchaseOrderId, x.DocumentId });

            builder.HasOne(p => p.PurchaseOrder)
                .WithMany(c => c.Documents)
                .HasForeignKey(bc => bc.PurchaseOrderId);

            builder.HasOne<Document>(x => x.Document)
                .WithMany()
                .HasForeignKey(c => c.DocumentId);

            builder.Property(x => x.IsActive).HasColumnOrder(100).HasDefaultValue(true);

        }
    }

    public class PurchaseOrderAttachedProductDocumentConfiguration : IEntityTypeConfiguration<PurchaseOrderAttachedProductDocument>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderAttachedProductDocument> builder)
        {
            builder.ToTable("PurchaseOrderAttachedProductDocuments");

            builder.HasKey(x => new { x.PurchaseOrderId, x.DocumentId });

            builder.HasOne(p => p.PurchaseOrder)
                .WithMany(c => c.AttachedProductDocuments)
                .HasForeignKey(bc => bc.PurchaseOrderId);

            builder.HasOne<ProductDocument>(x => x.ProductDocument)
                .WithMany()
                .HasForeignKey(c => new { c.PurchaseOrderProductId, c.DocumentId });

            builder.Property(x => x.IsActive).HasColumnOrder(100).HasDefaultValue(true);

        }
    }

    public class PurchaseOrderEventConfiguration : IEntityTypeConfiguration<PurchaseOrderEvent>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderEvent> builder)
        {
            builder.ToTable("PurchaseOrderEvents");

            builder.Property(x => x.EventId).UseIdentityColumn().HasColumnOrder(0);
            builder.HasKey(x => x.EventId);

            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(1);


            builder.Property(x => x.EventType).HasColumnOrder(20);
            builder.Property(p => p.Description).HasMaxLength(256).HasColumnOrder(21);

            builder.Property(x => x.ActionedBy).HasColumnOrder(30);
            builder.Property(x => x.ActionDate).HasColumnOrder(31);
        }
    }

    public class PurchaseOrderItemScheduleLineConfiguration : BaseEntityTypeConfiguration<PurchaseOrderItemScheduleLine>
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrderItemScheduleLine> builder)
        {
            builder.HasKey(x => new { x.PurchaseOrderId, x.PurchaseOrderItemId, x.Id });

            builder.ToTable("PurchaseOrderItemScheduleLines");

            builder.Property(x => x.PurchaseOrderId).HasColumnOrder(0);
            builder.Property(x => x.PurchaseOrderItemId).HasColumnOrder(1);

            builder.Property(x => x.RequestedGoodsReadyDate).HasColumnOrder(10);
            builder.Property(x => x.ConfirmedGoodsReadyDate).HasColumnOrder(11);
            builder.Property(x => x.RequestedDeliveryDate).HasColumnOrder(12);
            builder.Property(x => x.ConfirmedDeliveryDate).HasColumnOrder(13);

            builder.Property(x => x.ScheduleLineOrderQuantity).HasColumnOrder(21);
            builder.Property(x => x.ScheduleLineCommittedQuantity).HasColumnOrder(22);

            builder.Property(x => x.OrderQuantityUnit).HasColumnOrder(31);
            builder.Property(x => x.ScheduleLineOrderWeight).HasColumnOrder(32);

            builder.Property(x => x.PlaceOfLoadingId).HasMaxLength(8).HasColumnOrder(41);

            builder.HasOne<Address>(x => x.PlaceOfLoading)
                .WithMany()       // <---
                .HasForeignKey(c => c.PlaceOfLoadingId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CountryofLoadingCode).HasMaxLength(4).HasColumnOrder(42);

            builder.HasOne<Country>(x => x.CountryOfLoading)
                .WithMany()       // <---
                .HasForeignKey(c => c.CountryofLoadingCode).OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Description).HasMaxLength(250).HasColumnOrder(30);

            base.Configure(builder);
        }
    }


}
