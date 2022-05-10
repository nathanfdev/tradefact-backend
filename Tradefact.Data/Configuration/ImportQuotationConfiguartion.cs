using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{
    [Obsolete]

    public class ImportQuotationConfiguration : CosmoItemTypeConfiguration<ImportQuotation>
    {
        public override void Configure(EntityTypeBuilder<ImportQuotation> builder)
        {
            builder.ToTable("Quotations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DateDue).HasColumnType("datetime2");
            builder.Property(x => x.DateIssued).HasColumnType("datetime2");

            builder.Ignore(x => x.GoodsReady);

            builder.Property(x => x.IncoTerms);

            builder.Property(x => x.InsuranceValue).HasColumnType("decimal(18, 2)");
            // builder.Property(x => x.IsoCurrency).HasMaxLength(6);

            builder.OwnsOne<LoadModel>(x => x.FCL, a =>
            {
                a.Property(p => p.FT20);
                a.Property(p => p.FT40);
                a.Property(p => p.FTHHQ40);
                a.Property(p => p.FTHQ45);
            });

            builder.OwnsOne<LCLModel>(x => x.LCL, a =>
            {
                a.Property(p => p.Length);
                a.Property(p => p.Width);
                a.Property(p => p.Height);
                a.Property(p => p.Weight);
                a.Ignore(i => i.CBM);
            });


            builder.Property(x => x.LoadType);

            builder.Property(x => x.NumberOfItems).IsRequired(false);
            builder.Property(x => x.PlaceOfDispatch).HasMaxLength(50);
            builder.Property(x => x.PlaceOfLoading).HasMaxLength(50);
            builder.Property(x => x.PortOfDischarge).HasMaxLength(50);
            builder.Property(x => x.PortOfLoading).HasMaxLength(50);

            // builder.Ignore(x => x.Supplier);

            builder.Property(x => x.CurrencyConverted).HasMaxLength(50);
            builder.Property(x => x.TargetCurrency).HasMaxLength(50);

            builder.Property(x => x.TotalCost).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.TotalCostConverted).HasColumnType("decimal(18, 2)");

            builder.Property(x => x.TransitTime);

            // builder.HasMany(a => a.LineItems).WithOne(t => t.Quotation).HasForeignKey(t => t.QuotationId);


            base.Configure(builder);
        }
    }
}
