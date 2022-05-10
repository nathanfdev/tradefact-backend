using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{
    public class OrganisationTypeConfiguration: IEntityTypeConfiguration<OrganisationType>
    {
        public void Configure(EntityTypeBuilder<OrganisationType> builder)
        {
            builder.ToTable("OrganisationType");

            builder.HasKey(x => x.OrganisationTypeId);
            builder.Property(x => x.OrganisationTypeId).HasConversion<int>().HasColumnOrder(1);
            builder.Property(x => x.Name).HasMaxLength(32).HasColumnOrder(2);

            builder.HasMany(a => a.Organisations).WithOne(t => t.OrgansiationType).HasForeignKey(t => t.OrganisationTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }

}
