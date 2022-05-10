using Core.Models.External;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{
    public class ExternalPurchaseOrderConfiguration : BaseEntityTypeConfiguration<ExternalPurchaseOrder>
    {

        public override void Configure(EntityTypeBuilder<ExternalPurchaseOrder> builder)
        {
            builder.ToTable("ExternalPurchaseOrders", Schemas.Integrations);
            builder.HasKey(x => x.Id);

            builder.Property(p => p.ExternalID).HasMaxLength(128).HasColumnOrder(11);
            builder.Property(p => p.PurchaseOrderNumber).HasMaxLength(128).HasColumnOrder(12);
            builder.Property(p => p.Reference).HasMaxLength(128).HasColumnOrder(13);
            builder.Property(p => p.OrganisationId).HasColumnOrder(14);

            builder.Property(p => p.Status).HasMaxLength(30).HasColumnOrder(15);

            builder.Property(x => x.OrderDate).HasColumnOrder(20);
            builder.Property(x => x.GoodsReadyDate).HasColumnOrder(21);
            builder.Property(x => x.DateOfIssue).HasColumnOrder(22);
            builder.Property(p => p.PlaceOfIssue).HasMaxLength(128).HasColumnOrder(23);

            builder.Property(x => x.CurrencyRate).HasColumnOrder(30);
            builder.Property(x => x.CurrencyCode).HasMaxLength(16).HasColumnOrder(31);

            builder.Property(p => p.SubTotal).HasColumnType("decimal(18, 2)").HasColumnOrder(40);
            builder.Property(p => p.TotalTax).HasColumnType("decimal(18, 2)").HasColumnOrder(41);
            builder.Property(p => p.Total).HasColumnType("decimal(18, 2)").HasColumnOrder(42);

            builder.Property(x => x.SupplierId).HasMaxLength(36).HasColumnOrder(50);
            builder.Property(x => x.SupplierName).HasMaxLength(256).HasColumnOrder(51);

            builder.Property(x => x.Received).HasColumnOrder(60);
            builder.Property(x => x.Imported).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(61);
            builder.Property(x => x.ImportDate).HasColumnOrder(62);
            builder.Property(x => x.ImportUserId).HasColumnOrder(63);
            builder.Property(x => x.ImportUserName).HasMaxLength(64).HasColumnOrder(64);
            builder.Property(x => x.GenericProductId).HasColumnOrder(65);


            builder.Property(p => p.Tags).HasMaxLength(512).HasColumnOrder(70);
            builder.Property(p => p.PaymentTerms).HasMaxLength(512).HasColumnOrder(71);
            builder.Property(p => p.Source).HasMaxLength(16).HasColumnOrder(72);


            builder.HasMany(a => a.LineItems).WithOne(t => t.ExternalPurchaseOrder).HasForeignKey(t => t.ExternalPurchaseOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }

}
