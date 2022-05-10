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
    public class NewsFeedConfiguration : BaseEntityTypeConfiguration<NewsFeed>
    {

        public override void Configure(EntityTypeBuilder<NewsFeed> builder)
        {
            builder.ToTable("NewsFeeds");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type).HasMaxLength(32).HasColumnOrder(2);
            builder.Property(x => x.WeekNo).HasColumnOrder(3);

            builder.OwnsMany<NewsSection>(x => x.Sections, a =>
            {
                a.ToTable("NewsFeedSections");
                a.WithOwner().HasForeignKey(p => p.NewsFeedId);
                a.HasKey(x => new { x.NewsFeedId, x.Reference });

                a.Property(x => x.NewsFeedId).HasColumnOrder(1);
                a.Property(x => x.Reference).HasMaxLength(36).HasColumnOrder(2);

                a.Property(x => x.Title).HasMaxLength(128).HasColumnOrder(3);

                a.OwnsMany<NewsItem>(se => se.Items, se =>
                {
                    se.ToTable("NewsFeedItems");
                    se.WithOwner().HasForeignKey(ni => new { ni.NewsFeedId, ni.NewsSectionReference });

                    se.HasKey(s => new { s.NewsFeedId, s.NewsSectionReference, s.SeqNo  });

                    se.Property(x => x.NewsFeedId).HasColumnOrder(1);
                    se.Property(x => x.NewsSectionReference).HasMaxLength(36).HasColumnOrder(2);
                    se.Property(x => x.SeqNo).HasColumnOrder(3).ValueGeneratedNever();
                    se.Property(x => x.Text).HasColumnOrder(4);
                });
            });

            base.Configure(builder);
        }
    }
}
