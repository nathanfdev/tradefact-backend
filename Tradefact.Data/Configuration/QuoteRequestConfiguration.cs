using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tradefact.Data.Configuration
{
    public class QuotationRequestConfiguration : BaseEntityTypeConfiguration<QuotationRequest>
    {
        public override void Configure(EntityTypeBuilder<QuotationRequest> builder)
        {
            builder.ToTable("QuotationRequests");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Submitted).HasColumnType("datetime2").HasColumnOrder(100);
            builder.Property(x => x.Actioned).HasColumnType("datetime2").HasColumnOrder(101);

            builder.Property(x => x.State).HasConversion<int>().HasColumnOrder(102);

            builder.HasMany(a => a.Quotations).WithOne(t => t.QuotationRequest).HasForeignKey(f => f.QuotationRequestId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Shipments).WithOne(t => t.QuotationRequest).HasForeignKey(t => t.QuotationRequestId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }
}
