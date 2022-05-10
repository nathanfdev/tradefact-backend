using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class FreightMovementItemConfiguration: IEntityTypeConfiguration<FreightMovementItem>
    {
        public void Configure(EntityTypeBuilder<FreightMovementItem> builder)
        {
            builder.ToTable("FreightMovementItems");

            builder.HasKey(x => new { x.FreightMovementId, x.Id });

            builder.Property(x => x.ContainerTypeCode).HasMaxLength(4);
            builder.Property(x => x.HazardCode).HasMaxLength(4);

            builder.HasMany(a => a.CargoItems).WithOne(t => t.FreightMovementItem).HasForeignKey(t => new { t.FreightMovementId, t.FreightMovementItemId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
        }
    }

}
