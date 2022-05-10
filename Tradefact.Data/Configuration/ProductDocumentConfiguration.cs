using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class ProductDocumentConfiguration : IEntityTypeConfiguration<ProductDocument>
    {
        public void Configure(EntityTypeBuilder<ProductDocument> builder)
        {
            builder.ToTable("ProductDocuments");

            builder.HasKey(x => new { x.ProductId, x.DocumentId });

            //builder.HasOne(p => p.Product)
            //    .WithMany(c => c.Documents)
            //    .HasForeignKey(bc => bc.ProductId);

            builder.HasOne<Document>(x => x.Document)
                .WithMany()
                .HasForeignKey(c => c.DocumentId);

            builder.Property(x => x.IsActive).HasColumnOrder(100).HasDefaultValue(true);

        }
    }


}

