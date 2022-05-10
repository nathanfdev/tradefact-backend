using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    //public class ActivityItemConfiguration : BaseEntityTypeConfiguration<ActivityItem>
    //{
    //    public override void Configure(EntityTypeBuilder<ActivityItem> builder)
    //    {
    //        builder.ToTable("ActivityItems");

    //        builder.HasKey(x => x.Id);
    //        builder.Property(x => x.Reference).HasMaxLength(50);

    //        // builder.HasOne(a => a.Quotation).WithOne(t => t.ActivityItem).HasForeignKey<Shipment>(b => b.ActivityItemId);
    //        builder.HasOne(a => a.Shipment).WithOne(t => t.ActivityItem).HasForeignKey<Shipment>(b => b.ActivityItemId);
    //        // builder.HasOne(a => a.PO).WithOne(t => t.ActivityItem).HasForeignKey<PurchaseOrder>(b => b.ActivityItemId);

    //        base.Configure(builder);
    //    }
    //}

    //public class PurchaseOrderConfiguration : BaseEntityTypeConfiguration<PurchaseOrder>
    //{
    //    public override void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    //    {
    //        builder.ToTable("PurchaseOrders");

    //        builder.HasKey(x => x.Id);
    //        builder.Property(x => x.Reference).HasMaxLength(50);

    //        builder.HasMany(a => a.Items).WithOne(t => t.PurchaseOrder).HasForeignKey(b => b.PurchaseOrderId);

    //        base.Configure(builder);
    //    }
    //}

    //public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
    //{
    //    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    //    {
    //        builder.ToTable("PurchaseOrderDetails");

    //        builder.HasKey(x => new { x.PurchaseOrderId, x.LineNo });
    //        builder.Property(x => x.Message).HasMaxLength(250);
    //    }
    //}


}
