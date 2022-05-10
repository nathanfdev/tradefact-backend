using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class ProductVariantConfiguration : BaseEntityTypeConfiguration<ProductVariant>
    {
        public override void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.Property(x => x.SupplierId).HasColumnOrder(90);

            builder.Property(x => x.Name).HasMaxLength(250).HasColumnOrder(100);
            builder.Property(x => x.SKU).HasMaxLength(50).HasColumnOrder(101);
            builder.Property(x => x.Description).HasMaxLength(512).HasColumnOrder(102);
            builder.Property(x => x.Reference).HasMaxLength(256).HasColumnOrder(103);

            builder.Property(x => x.HsCode).HasMaxLength(250).HasColumnOrder(110);
            builder.Property(x => x.GoodsType).HasMaxLength(250).HasColumnOrder(111);
            builder.Property(x => x.Nickname).HasMaxLength(250).HasColumnOrder(112);
            builder.Property(x => x.Packing).HasMaxLength(50).HasColumnOrder(113);

            builder.Property(x => x.UnitsPerPackage).HasColumnOrder(122);


            builder.Property(x => x.HazardClass).HasMaxLength(32).HasColumnOrder(130);
            builder.Property(x => x.HazardDescription).HasMaxLength(50).HasColumnOrder(131);
            builder.Property(x => x.HazardousContents).HasMaxLength(50).HasColumnOrder(132);
            builder.Property(x => x.HazardNotes).HasMaxLength(512).HasColumnOrder(133);

            builder.Property(x => x.HazardDocumentId).HasMaxLength(8).HasColumnOrder(134);

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

            base.Configure(builder);
        }
    }
}
