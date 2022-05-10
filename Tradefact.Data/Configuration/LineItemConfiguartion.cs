using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class LineItemModelConfiguration : CosmoItemTypeConfiguration<LineItemModel>
    {
        public override void Configure(EntityTypeBuilder<LineItemModel> builder)
        {
            builder.ToTable("QuotationLineItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity);
            builder.Property(x => x.Description).HasMaxLength(512);
            builder.Property(x => x.Amount).HasColumnType("decimal(18, 2)");


            //builder.Property(p => p.Containers).HasConversion(
            //    v => JsonSerializer.Serialize(v, default),
            //    v => JsonSerializer.Deserialize<List<string>>(v, default)
            //);

            base.Configure(builder);
        }
    }
}
