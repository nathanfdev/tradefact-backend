using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tradefact.Data.Configuration
{

//    public class PartnerConfiguration : BaseEntityTypeConfiguration<Partner>
//    {
//        public override void Configure(EntityTypeBuilder<Partner> builder)
//        {
//            builder.ToTable("Partners");
//            builder.HasKey(x => x.Id);
//            builder.Property(x => x.AccountNumber).HasMaxLength(250);
//            builder.Property(x => x.Name).HasMaxLength(250);
//            builder.Property(x => x.BankName).HasMaxLength(250);
//            builder.Property(x => x.ContactEmail).HasMaxLength(250);
//            builder.Property(x => x.ContactName).HasMaxLength(250);
//            builder.Property(x => x.ContactTelephone).HasMaxLength(50);
//            builder.Property(x => x.Currency).HasMaxLength(3);
//            builder.Property(x => x.CustomsFee).HasColumnType("decimal(18, 2)");
//            builder.Property(x => x.DocumentsFee).HasColumnType("decimal(18, 2)");
//            builder.Property(x => x.PartnerCode).HasMaxLength(250);
//            builder.Property(x => x.SortCode).HasMaxLength(34);
//            builder.Property(x => x.SwiftCode).HasMaxLength(12);
//            builder.Property(x => x.TaxRate).HasColumnType("decimal(18, 2)");
//            builder.Property(x => x.TosDocumentId).HasMaxLength(250);
//            builder.Property(x => x.Website).HasMaxLength(250);

////            builder.OwnsOne(x => x.Address, a =>
////            {
////                a.ToTable("PartnerAddresses");
////                a.Property(p => p.AddressLine1).HasMaxLength(250);
////                a.Property(p => p.AddressLine2).HasMaxLength(250);
////                a.Property(p => p.AddressLine3).HasMaxLength(250);
////                a.Property(p => p.AddressLine4).HasMaxLength(250);
////                a.Property(p => p.PostalCode).HasMaxLength(50);
////                a.Property(p => p.Province).HasMaxLength(50);
////                a.Property(p => p.City).HasMaxLength(50);
//////                a.Property(p => p.Country).HasMaxLength(50);

////                a.Ignore(x => x.Id);
////                a.Ignore(x => x.IsActive);
////                a.Ignore(x => x.Province);
////                a.Ignore(x => x.Country);
////                //a.Ignore(x => x.ETag);
////                //a.Ignore(x => x.PartitionKeyValue);
////                //a.Ignore(x => x.Type);
////            });


//            // builder.HasMany(a => a.Shipments).WithOne(t => t.Partner).HasForeignKey(t => t.PartnerId).IsRequired(true);

//            //builder.HasMany(a => a.Rates).WithOne(t => t.Partner).HasForeignKey(t => t.PartnerId);
//            //builder.HasMany(a => a.PartnerRateHaulages).WithOne(t => t.Partner).HasForeignKey(t => t.PartnerId);

//            base.Configure(builder);
//        }
//    }
}
