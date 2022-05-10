using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class ProductConfiguration : BaseEntityTypeConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyId).HasColumnOrder(90);

            builder.Property(x => x.Name).HasMaxLength(250).HasColumnOrder(100);
            builder.Property(x => x.SKU).HasMaxLength(150).HasColumnOrder(101);
            builder.Property(x => x.Description).HasMaxLength(512).HasColumnOrder(102);
            builder.Property(x => x.Reference).HasMaxLength(256).HasColumnOrder(103);


            builder.Property(x => x.Barcode).HasMaxLength(250).HasColumnOrder(109);
            builder.Property(x => x.HsCode).HasMaxLength(250).HasColumnOrder(110);
            builder.Property(x => x.GoodsType).HasMaxLength(250).HasColumnOrder(111);
            builder.Property(x => x.Nickname).HasMaxLength(250).HasColumnOrder(112);
            builder.Property(x => x.Packing).HasMaxLength(50).HasColumnOrder(113);

            builder.Property(x => x.Stackable).HasColumnOrder(120);
            builder.Property(x => x.Rotatable).HasColumnOrder(121);
            builder.Property(x => x.UnitsPerPackage).HasColumnOrder(122);


            builder.Property(x => x.HazardClass).HasMaxLength(32).HasColumnOrder(130);
            builder.Property(x => x.HazardDescription).HasMaxLength(50).HasColumnOrder(131);
            builder.Property(x => x.HazardousContents).HasMaxLength(50).HasColumnOrder(132);
            builder.Property(x => x.HazardNotes).HasMaxLength(512).HasColumnOrder(133);

            builder.Property(x => x.HazardDocumentId).HasMaxLength(8).HasColumnOrder(134);

            builder.Property(x => x.LithiumBatteryPacking).HasMaxLength(8).HasColumnOrder(135);
            builder.Property(x => x.MagneticFieldContained).HasColumnOrder(136);


            builder.OwnsOne<ImportProductDimensions>(x => x.Dimensions, a =>
            {
                a.Property(p => p.Length).HasColumnOrder(140);
                a.Property(p => p.Width).HasColumnOrder(141);
                a.Property(p => p.Height).HasColumnOrder(142);
                a.Property(p => p.Weight).HasColumnOrder(143);
                a.Property(p => p.weightMeasurement).HasMaxLength(16).HasColumnOrder(144);
                a.Property(p => p.Scale).HasMaxLength(16).HasColumnOrder(145);
            });

            builder.HasOne(x => x.HazardDocument).WithMany().HasForeignKey(x => x.HazardDocumentId).OnDelete(DeleteBehavior.Restrict); ;

            builder.HasMany(a => a.Variants).WithOne(t => t.Product).HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Identifier, a =>
            {
                a.Property(p => p.GTIN).HasMaxLength(20).HasColumnOrder(160);
                a.Property(p => p.UPC).HasMaxLength(20).HasColumnOrder(161);
                a.Property(p => p.EAN).HasMaxLength(20).HasColumnOrder(162);
                a.Property(p => p.JAN).HasMaxLength(20).HasColumnOrder(163);
                a.Property(p => p.ASIN).HasMaxLength(20).HasColumnOrder(164);
                a.Property(p => p.ISBN).HasMaxLength(20).HasColumnOrder(165);
                a.Property(p => p.MPN).HasMaxLength(20).HasColumnOrder(166);
                a.Property(p => p.ePID).HasMaxLength(20).HasColumnOrder(167);
                a.Property(p => p.GPC).HasMaxLength(20).HasColumnOrder(168);
            });

            builder.Property(x => x.StockQuantity).HasColumnOrder(170);
            builder.Property(x => x.MinStockQuantity).HasColumnOrder(171);
            builder.Property(x => x.NotifyStockQuantityBelow).HasColumnOrder(172);
            builder.Property(x => x.OrderQuantityMaximum).HasColumnOrder(174);
            builder.Property(x => x.IncomingStockQuantity).HasColumnOrder(174);

            builder.Property(x => x.Tags).HasMaxLength(512).HasColumnOrder(200);

            builder.Property(x => x.DataComplete).HasDefaultValue(false).HasColumnOrder(210);

            builder.HasMany(a => a.Suppliers).WithOne(t => t.Product).HasForeignKey(t => t.ProductId);

            base.Configure(builder);
        }
    }


}

