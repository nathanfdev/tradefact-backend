using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flare.Data.Configuration
{
    public abstract class BaseEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity<TBase>
    {
        public virtual string SchemaName { get; } = "dbo";

        protected int MaxReferenceLength => 32;

        public virtual void Configure(EntityTypeBuilder<TBase> builder)
        {
            builder.Property(x => x.Id).HasColumnOrder(2);
            builder.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

            builder.Ignore(x => x.CreationDate);
            builder.Ignore(x => x.LastModifiedOn);

            builder.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(9991);
            builder.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(9992);

            builder.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(9993);
            builder.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(9994);

            builder.Ignore(x => x.Timestamp);
        }
    }

}
