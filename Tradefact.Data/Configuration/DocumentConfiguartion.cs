using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{

    public class DocumentConfiguration: BaseEntityTypeConfiguration<Document>
    {
        public override void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => new { x.Id });

            builder.ToTable("Documents");

            builder.Property(x => x.Name).HasMaxLength(128).HasColumnOrder(20);
            builder.Property(x => x.Extension).HasMaxLength(8).HasColumnOrder(21);
            builder.Property(x => x.Description).HasMaxLength(256).HasColumnOrder(22);
            builder.Property(x => x.BlobUrl).HasMaxLength(512).HasColumnOrder(23);
            builder.Property(x => x.CompanyId).HasColumnOrder(24);

            builder.Property(x => x.IsRichText).HasColumnOrder(26);
            builder.Property(x => x.RichTextData).HasColumnOrder(27);

            builder.Property(x => x.ThumbnailGenerated).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(28);
            builder.Property(x => x.ThumbnailUrl).HasMaxLength(512).HasColumnOrder(29);

            builder.Property(x => x.DateUploaded).HasColumnType("datetime2").HasColumnOrder(30);
            builder.Property(x => x.CreationDate).HasColumnType("datetime2").HasColumnOrder(31);

            base.Configure(builder);
        }
    }
}
