using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data.Configuration
{

    public class ShipmentConfiguration : BaseEntityTypeConfiguration<Shipment>
    {
        public override void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments");

            builder.HasKey(x => new { x.Id });

            builder.HasIndex(x => new { x.PartnershipId, x.Id });

            builder.Property(x => x.PartnershipId).HasColumnOrder(2);
            builder.Property(x => x.FreightMovementId).HasColumnOrder(3);
            builder.Property(x => x.QuotationRequestId).HasColumnOrder(4);

            builder.Property(x => x.ShipmentType).HasColumnOrder(11);
            builder.Property(x => x.IncoTerms).HasColumnOrder(12);
            builder.Property(x => x.LoadType).HasColumnOrder(13);
            builder.Property(x => x.Status).HasColumnOrder(14);
            builder.Property(x => x.ShipmentName).HasMaxLength(256).HasColumnOrder(15);


            builder.Property(x => x.Booked).HasColumnOrder(20);
            builder.Property(x => x.BookedDate).HasColumnOrder(21);

            builder.Property(x => x.PlaceOfLoadingId).HasMaxLength(8).HasColumnOrder(30);

            builder.Property(x => x.EstimatedCollectionDate).HasColumnOrder(31);
            builder.Property(x => x.Collected).HasColumnOrder(32);
            builder.Property(x => x.CollectionDate).HasColumnOrder(33);

            builder.Property(x => x.InTransit).HasColumnOrder(40);
            builder.Property(x => x.InTransitDate).HasColumnOrder(41);

            builder.Property(x => x.SCAC).HasMaxLength(32).HasColumnOrder(50);
            builder.Property(x => x.IMO).HasMaxLength(32).HasColumnOrder(51);
            builder.Property(x => x.VesselName).HasMaxLength(128).HasColumnOrder(52);
            builder.Property(x => x.BillofLadingNumber).HasMaxLength(36).HasColumnOrder(53);

            builder.Property(x => x.TrackinformationAdded).HasColumnOrder(60);
            builder.Property(x => x.ShipmentTrackAvailable).HasColumnOrder(61);

            builder.Property(x => x.PortOfLoadingId).HasMaxLength(8).HasColumnOrder(70);
            builder.Property(x => x.DepartedPOL).HasColumnOrder(71);
            builder.Property(x => x.DepartedPOLDate).HasColumnOrder(72);
            builder.Property(x => x.ETD).HasColumnOrder(73);

            builder.Property(x => x.ETA).HasColumnOrder(80);
            builder.Property(x => x.PortOfDischargeId).HasMaxLength(8).HasColumnOrder(81);
            builder.Property(x => x.ArrivedPOD).HasColumnOrder(82);
            builder.Property(x => x.ArrivedPODDate).HasColumnOrder(83);
            builder.Property(x => x.PlaceOfDispatchId).HasMaxLength(8).HasColumnOrder(84);

            builder.Property(x => x.InCustoms).HasColumnOrder(90);
            builder.Property(x => x.IssueAtCustoms).HasColumnOrder(91);
            builder.Property(x => x.IssueAtCustomsDate).HasColumnOrder(92);
            builder.Property(x => x.IssueAtCustomCleared).HasColumnOrder(93);
            builder.Property(x => x.CustomsClearence).HasColumnOrder(94);
            builder.Property(x => x.CustomsClearenceDate).HasColumnOrder(95);

            builder.Property(x => x.EstimatedDeliveryDate).HasColumnOrder(100);
            builder.Property(x => x.Delivered).HasColumnOrder(101);
            builder.Property(x => x.DeliveryDate).HasColumnOrder(102);


            builder.Property(x => x.IsRescheduled).HasColumnOrder(110);
            builder.Property(x => x.Rescheduled).HasColumnOrder(111);
            builder.Property(x => x.LastRescheduleTime).HasColumnOrder(112);

            builder.Property(x => x.Stage).HasColumnOrder(120);
            builder.Property(x => x.Latitude).HasColumnOrder(121);
            builder.Property(x => x.Longitude).HasColumnOrder(122);

            builder.Property(x => x.Tags).HasMaxLength(512).HasColumnOrder(130);
            builder.Property(x => x.Notes).HasMaxLength(512).HasColumnOrder(131);
            builder.Property(x => x.Route).HasColumnOrder(132);


            //builder.OwnsMany<PurchaseOrder>(x => x.PurchaseOrders, a =>
            //{
            //    a.ToTable("PurchaseOrders");
            //    a.HasKey(p => new { p.Id });

            //    a.OwnsMany(b => b.PurchaseOrderItems, c => {
            //        c.ToTable("PurchaseOrderItems");
            //      });
            //});


            builder.OwnsMany(x => x.Equipment, a =>
            {
                a.ToTable("EquipmentAllocations");
                a.HasKey(p => new { p.ShipmentId, p.Id });
            });


            builder.HasOne<Address>(x => x.PlaceOfLoading)
                .WithMany()       // <---
                .HasForeignKey(c => c.PlaceOfLoadingId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Address>(x => x.PlaceOfDispatch)
                .WithMany()       // <---
                .HasForeignKey(c => c.PlaceOfDispatchId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Location>(x => x.PortOfLoading)
                .WithMany()       // <---
                .HasForeignKey(c => c.PortOfLoadingId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Location>(x => x.PortOfDischarge)
                .WithMany()       // <---
                .HasForeignKey(c => c.PortOfDischargeId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Route).HasColumnType("nvarchar(max)");

            base.Configure(builder);
        }
    }

    public class ShipmentDocumentConfiguration : IEntityTypeConfiguration<ShipmentDocument>
    {
        public void Configure(EntityTypeBuilder<ShipmentDocument> builder)
        {
            builder.ToTable("ShipmentDocuments");

            builder.HasKey(x => new { x.ShipmentId, x.DocumentId });

            builder.HasOne(p => p.Shipment)
                .WithMany(c => c.Documents)
                .HasForeignKey(bc => bc.ShipmentId);

            builder.HasOne<Document>(x => x.Document)
                .WithMany()
                .HasForeignKey(c => c.DocumentId);

            builder.Property(x => x.IsActive).HasColumnOrder(100).HasDefaultValue(true);

        }
    }

    public class ShipmentEventConfiguration : IEntityTypeConfiguration<ShipmentEvent>
    {
        public void Configure(EntityTypeBuilder<ShipmentEvent> builder)
        {
            builder.ToTable("ShipmentEvents");

            builder.HasKey(x => new { x.ShipmentId, x.EquipmentItemId, x.EventId });

            builder.HasOne(p => p.Shipment)
                .WithMany(c => c.TrackingEvents)
                .HasForeignKey(bc => bc.ShipmentId);

            builder.Property(x => x.ShipmentId).HasColumnOrder(1);
            builder.Property(x => x.TrackingNumber).HasMaxLength(32).HasColumnOrder(2);
            builder.Property(x => x.EquipmentItemId).HasMaxLength(32).HasColumnOrder(3);
            builder.Property(x => x.EventId).HasColumnOrder(4);

            builder.Property(x => x.TimeOfEvent).HasColumnType("datetime2").HasColumnOrder(10);

            builder.Property(x => x.Voyage).HasMaxLength(256).HasColumnOrder(11);
            builder.Property(x => x.Activity).HasMaxLength(64).HasColumnOrder(12);
            builder.Property(x => x.Information).HasMaxLength(256).HasColumnOrder(13); 
            builder.Property(x => x.LocationOfEvent).HasMaxLength(256).HasColumnOrder(14);
        }
    }


}
