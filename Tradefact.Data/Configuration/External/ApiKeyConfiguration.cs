using Core.Models.External;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{
    public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
    {

        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.ToTable("ApiKeys", Schemas.Integrations);
            builder.HasKey(x => x.Key);

            builder.Property(p => p.Key).HasMaxLength(36).HasColumnOrder(11);
            builder.Property(p => p.OrganisationId).HasMaxLength(36).HasColumnOrder(12);
        }
    }

}
