using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class ShipmentTypeConfiguration: IEntityTypeConfiguration<ShipmentType>
    {
        public void Configure(EntityTypeBuilder<ShipmentType> builder)
        {
            builder.ToTable("ShipmentType");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasConversion<int>();
            builder.Property(x => x.Name).HasMaxLength(32);

            builder.HasMany(a => a.Carriers).WithOne(t => t.ShipmentType).HasForeignKey(t => t.ShipmentTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
