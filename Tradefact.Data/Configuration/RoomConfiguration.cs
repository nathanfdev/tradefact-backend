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
    public class RoomConfiguration : BaseEntityTypeConfiguration<Room>
    {
        public override void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms", Schemas.Collaboration);
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Type).HasColumnOrder(20);
            builder.Property(x => x.Name).HasMaxLength(250).HasColumnOrder(21);
            builder.Property(x => x.IsOpen).HasColumnOrder(22);
            builder.Property(x => x.IsAlert).HasColumnOrder(23);
            builder.Property(x => x.UnreadCount).HasColumnOrder(24);

            builder.HasMany(a => a.Messages).WithOne(t => t.Room).HasForeignKey(t => t.RoomId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }

    public class MessageConfiguration : BaseEntityTypeConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages", Schemas.Collaboration);
            builder.HasKey(x => new {x.RoomId, x.Id });
            builder.Property(x => x.Type).HasMaxLength(8);

            builder.Ignore(x => x.PinnedBy);
            builder.Ignore(x => x.WasEdited);
            builder.Ignore(x => x.Reactions);
            builder.Ignore(x => x.Starred);
            builder.Ignore(x => x.Attachments);

            builder.OwnsOne(x => x.PostedBy, a =>
            {
                a.Property(p => p.DisplayName).HasMaxLength(128);
                a.Property(p => p.Avatar).HasMaxLength(256);
            });

            base.Configure(builder);
        }
    }

}
