using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class QuotationConfiguration : BaseEntityTypeConfiguration<Quotation>
    {
        public override void Configure(EntityTypeBuilder<Quotation> builder)
        {
            builder.ToTable("Quotations");

            builder.HasKey(x => new { x.Id });
            builder.HasIndex(x => new { x.QuotationRequestId, x.Id });

            builder.Property(x => x.QuotationRequestId).HasColumnOrder(2);
            builder.Property(x => x.ShipmentId).HasColumnOrder(3);
            builder.Property(x => x.FreightMovementId).HasColumnOrder(4);

            builder.Property(x => x.Revision).HasColumnOrder(5);
            builder.Property(x => x.LoadType).HasColumnOrder(6);

            builder.Property(x => x.Booked).HasColumnOrder(11);
            builder.Property(x => x.Reference).HasMaxLength(128).HasColumnOrder(12);
            builder.Property(x => x.QuoteNumber).HasMaxLength(32).HasColumnOrder(13);
            builder.Property(x => x.IssueDate).HasColumnOrder(14);
            builder.Property(x => x.ExpiryDate).HasColumnOrder(15);
            builder.Property(x => x.QuoteStatus).HasColumnOrder(16);

            builder.Property(x => x.ContactName).HasMaxLength(128).HasColumnOrder(20);
            builder.Property(x => x.ContactReference).HasMaxLength(128).HasColumnOrder(21);
            builder.Property(x => x.Sent).HasColumnOrder(22);
            builder.Property(x => x.SentByEmail).HasColumnOrder(23);

            builder.Property(x => x.CurrencyId).HasMaxLength(16).HasColumnOrder(190);
            builder.Property(x => x.ExchangeRate).HasColumnOrder(191);
            builder.Property(x => x.InverseExchangeRate).HasColumnOrder(192);

            builder.Property(x => x.TotalQuantity).HasColumnOrder(260);

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

            builder.Property(x => x.BaseCurrencyTotalDiscountAmount).HasColumnOrder(260);

            builder.OwnsMany(x => x.Schedules, a =>
            {
                a.ToTable("QuotationAvailableSchedules");


                a.OwnsOne(x => x.POL, a =>
                {
                    a.Property(p => p.Code).HasMaxLength(6);
                    a.Property(p => p.Name).HasMaxLength(50);
                });
                a.OwnsOne(x => x.POD, a =>
                {
                    a.Property(p => p.Code).HasMaxLength(6);
                    a.Property(p => p.Name).HasMaxLength(50);
                });
                a.OwnsOne(x => x.Vessel, a =>
                {
                    a.Property(p => p.IMO).HasMaxLength(6);
                    a.Property(p => p.Name).HasMaxLength(50);
                });
                a.OwnsOne(x => x.Route, a =>
                {
                    a.Property(p => p.Code).HasMaxLength(6);
                    a.Property(p => p.Name).HasMaxLength(50);
                });
            });

            builder.Property(x => x.PaymentTerms).HasColumnType("nvarchar(max)").HasColumnOrder(299);
            builder.Property(x => x.Routes).HasColumnType("nvarchar(max)").HasColumnOrder(300);
            builder.Property(x => x.Notes).HasColumnType("nvarchar(max)").HasColumnOrder(301);
            builder.Property(x => x.TermsAndConditions).HasColumnType("nvarchar(max)").HasColumnOrder(302);
            builder.Property(x => x.DetailedTermsAndConditions).HasColumnType("nvarchar(max)").HasColumnOrder(303);

            builder.HasMany(a => a.FreightCharges).WithOne(p => p.Quotation).HasForeignKey(c => new { c.QuotationId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(a => a.OriginCharges).WithOne(p => p.Quotation).HasForeignKey(c => new { c.QuotationId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(a => a.DestinationCharges).WithOne(p => p.Quotation).HasForeignKey(c => new { c.QuotationId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(a => a.AdditionalCharges).WithOne(p => p.Quotation).HasForeignKey(c => new { c.QuotationId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }

    public class QuotationChargeItemConfiguration : IEntityTypeConfiguration<QuotationChargeItem>
    {
        public void Configure(EntityTypeBuilder<QuotationChargeItem> builder)
        {
            builder.ToTable("QuotationChargeItems");

            builder.HasKey(x => new { x.QuotationId, x.LineId });

            builder.Property(x => x.LineId).HasColumnOrder(2);
            builder.Property(x => x.Seq).HasColumnOrder(3);

            builder.Property(p => p.ServiceId).HasMaxLength(32).HasColumnOrder(4);
            builder.Property(p => p.Description).HasMaxLength(256).HasColumnOrder(5);

            builder.HasDiscriminator<QuotationChargeTypeEnum>("QuotationChargeTypeId")
                .HasValue<FreightCharge>(QuotationChargeTypeEnum.FREIGHT)
                .HasValue<OriginCharge>(QuotationChargeTypeEnum.ORIGIN)
                .HasValue<DestinationCharge>(QuotationChargeTypeEnum.DESTINATION)
                .HasValue<AdditionalCharge>(QuotationChargeTypeEnum.ADDITIONAL);

            builder.Property(p => p.Quantity).HasColumnOrder(200);
            builder.Property(p => p.UnitPrice).HasColumnOrder(201);
            builder.Property(p => p.UnitPriceIncludesTax).HasColumnOrder(202);
            builder.Property(p => p.TaxRate).HasColumnOrder(203);
            builder.Property(p => p.Margin).HasColumnOrder(204);

            builder.OwnsOne<Total>(x => x.Total, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(250);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(251);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(252);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(253);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(254);
            });

            builder.OwnsOne<Total>(x => x.BaseCurrency, a =>
            {
                a.Property(p => p.CurrencyId).HasMaxLength(12).HasColumnOrder(200);

                a.Property(p => p.NetAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(201);
                a.Property(p => p.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(202);
                a.Property(p => p.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(203);
                a.Property(p => p.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnOrder(204);
            });
        }
    }
}
