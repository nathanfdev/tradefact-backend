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
    public class OrganisationConfiguration : BaseEntityTypeConfiguration<Organisation>
    {
        public override void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.ToTable("Organisations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrganisationTypeId).HasColumnOrder(2);
            builder.Property(x => x.ParentId).HasColumnOrder(3);

            builder.Property(x => x.Name).HasMaxLength(250).HasColumnOrder(11);
            builder.Property(x => x.ContactName).HasMaxLength(250).HasColumnOrder(12);
            builder.Property(x => x.ContactEmail).HasMaxLength(250).HasColumnOrder(13);
            builder.Property(x => x.ContactTelephone).HasMaxLength(50).HasColumnOrder(14);


            builder.Property(x => x.TaxId).HasColumnOrder(15);
            builder.Property(x => x.PaymentTerms).HasColumnOrder(16);
            

            builder.Property(x => x.PlanInvitesAvailable).HasColumnOrder(17);
            builder.Property(x => x.InvitesIssued).HasColumnOrder(18);
            builder.Property(x => x.InvitesActioned).HasColumnOrder(19);
            builder.Property(x => x.Currency).HasDefaultValue("USD").HasColumnOrder(20);

            builder.OwnsOne(x => x.Bank, a =>
            {
                a.Property(p => p.BankIdentifierCode).HasMaxLength(16).HasColumnOrder(30);
                a.Property(p => p.IBAN).HasMaxLength(32).HasColumnOrder(31);
                a.Property(p => p.SortCode).HasMaxLength(8).HasColumnOrder(32);
                a.Property(p => p.RoutingNo).HasMaxLength(16).HasColumnOrder(33);
                a.Property(p => p.AccountNo).HasMaxLength(32).HasColumnOrder(34);
                a.Property(p => p.AccountName).HasMaxLength(32).HasColumnOrder(35);
                a.Property(p => p.IFSC).HasMaxLength(32).HasColumnOrder(36);
                a.Property(p => p.BSB).HasMaxLength(16).HasColumnOrder(37);
            });

            builder.Property(x => x.TCs).HasColumnOrder(40);

            builder.OwnsMany<OrganisationContact>(x => x.Contacts, a =>
            {
                a.ToTable("Contacts");
                a.WithOwner().HasForeignKey(p => p.OrganisationId);
                a.HasKey(x => new { x.OrganisationId, x.Id });
                a.Property(x => x.OrganisationId).HasColumnOrder(2);
                a.Property(x => x.IsActive).HasColumnOrder(4);
                a.Property(x => x.FullName).HasMaxLength(250).HasColumnOrder(31);
                a.Property(x => x.FirstName).HasMaxLength(250).HasColumnOrder(32);
                a.Property(x => x.MiddleName).HasMaxLength(250).HasColumnOrder(33);
                a.Property(x => x.LastName).HasMaxLength(250).HasColumnOrder(34);
                a.Property(x => x.Title).HasMaxLength(250).HasColumnOrder(35);
                a.Property(x => x.Salutation).HasMaxLength(250).HasColumnOrder(36);
                a.Property(x => x.Department).HasMaxLength(250).HasColumnOrder(37);
                a.Property(x => x.Notes).HasMaxLength(250).HasColumnOrder(38);
                a.Property(x => x.IsDefault).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(3);


                a.Property(x => x.Id).HasColumnOrder(1);
                a.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                a.Property(x => x.CreationDate).HasColumnOrder(998);
                a.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                a.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                a.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                a.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                a.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                a.Property(x => x.Timestamp).HasColumnOrder(1005);


                a.OwnsMany<OrganisationContactEmailAddress>(e => e.Email, ce =>
                {
                    ce.ToTable("ContactEmails");
                    ce.WithOwner().HasForeignKey(em => new { em.OrganisationId, em.ContactId });
                    ce.HasKey(em => new { em.ContactId, em.Id });
                    ce.Property(cm => cm.Email).HasMaxLength(250);
                    ce.Property(x => x.IsDefault).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever();

                    ce.Property(x => x.Id).HasColumnOrder(1);
                    ce.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                    ce.Property(x => x.CreationDate).HasColumnOrder(998);
                    ce.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                    ce.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                    ce.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                    ce.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                    ce.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                    ce.Property(x => x.Timestamp).HasColumnOrder(1005);
                });

                a.OwnsMany<OrganisationContactPhone>(e => e.Phone, ce =>
                {
                    ce.ToTable("ContactPhoneNumbers");
                    ce.WithOwner().HasForeignKey(em => new { em.OrganisationId, em.ContactId });
                    ce.HasKey(em => new { em.ContactId, em.Id });
                    ce.Property(cm => cm.AreaCode).HasMaxLength(8);
                    ce.Property(cm => cm.CountryCode).HasMaxLength(8);
                    ce.Property(cm => cm.Number).HasMaxLength(32);
                    ce.Property(x => x.IsDefault).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever();

                    ce.Property(x => x.Id).HasColumnOrder(1);
                    ce.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                    ce.Property(x => x.CreationDate).HasColumnOrder(998);
                    ce.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                    ce.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                    ce.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                    ce.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                    ce.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                    ce.Property(x => x.Timestamp).HasColumnOrder(1005);

                });

            });

            builder.OwnsMany<OrganisationNote>(x => x.Notes, a =>
            {
                a.ToTable("OrganisationNotes");
                a.WithOwner().HasForeignKey(p => p.OrganisationId);
                a.HasKey(x => new { x.OrganisationId, x.Id });
                a.Property(x => x.Note);

                a.Property(x => x.Id).HasColumnOrder(1);
                a.Property(x => x.IsActive).HasColumnName("Active").HasDefaultValue(true).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(10);

                a.Property(x => x.CreationDate).HasColumnOrder(998);
                a.Property(x => x.LastModifiedOn).HasColumnOrder(999);

                a.Property(x => x.CreatedByUser).HasMaxLength(128).HasColumnOrder(1001);
                a.Property(x => x.CreationDateInternal).HasColumnType("datetime2").HasColumnOrder(1002);
                a.Property(x => x.LastChangeUser).HasMaxLength(128).HasColumnOrder(1003);
                a.Property(x => x.LastModifiedOnInternal).HasColumnType("datetime2").HasColumnOrder(1004);

                a.Property(x => x.Timestamp).HasColumnOrder(1005);
            });

            builder.Property(x => x.TaxId).HasMaxLength(32);

            builder.Property(x => x.GenericSKUEnabled).HasDefaultValue(false).HasColumnType("bit").ValueGeneratedNever().HasColumnOrder(60); ;

            builder.Property(e => e.OrganisationTypeId).HasConversion<int>();

            builder.HasMany(a => a.Addresses).WithOne(t => t.Organisation).HasForeignKey(t => new { t.OrganisationId }).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Products).WithOne(t => t.Company).HasForeignKey(t => t.CompanyId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.ProductVariants).WithOne(t => t.Supplier).HasForeignKey(t => t.SupplierId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Directory).WithOne(t => t.Parent).HasForeignKey(t => t.ParentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.ProductsSupplied).WithOne(t => t.Supplier).HasForeignKey(t => t.SupplierId);

            builder.HasMany(a => a.FreightMovements).WithOne(t => t.Company).HasForeignKey(t => t.CompanyId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Directory).WithOne(t => t.Parent).HasForeignKey(t => t.ParentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.LogisticsProviders).WithOne(t => t.Client).HasForeignKey(t => t.ClientId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.LogisticsClients).WithOne(t => t.Provider).HasForeignKey(t => t.ProviderId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Documents).WithOne(t => t.Company).HasForeignKey(t => t.CompanyId).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }
}
