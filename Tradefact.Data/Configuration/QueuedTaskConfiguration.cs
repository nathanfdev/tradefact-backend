using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class QueuedTaskConfiguration : BaseEntityTypeConfiguration<QueuedTask>
    {
        public override void Configure(EntityTypeBuilder<QueuedTask> builder)
        {
            builder.ToTable("QueuedTask");

            builder.HasKey(x => new { x.Id });
            builder.Property(x => x.Status).HasColumnOrder(10);
            builder.Property(x => x.Description).HasMaxLength(256).HasColumnOrder(11);

            builder.Property(x => x.Payload).HasColumnOrder(20);
            builder.Property(x => x.Result).HasColumnOrder(21);
            
            base.Configure(builder);
        }
    }



}
