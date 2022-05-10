using Core.Interfaces;
using Core.Models;
using Flare.Data.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Flare.Data
{
    public class FlareDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IUserResolverService _userResolverService;

        public FlareDbContext(DbContextOptions<FlareDbContext> options, IUserResolverService userResolverService) : base(options)
        {
            this._userResolverService = userResolverService;
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<InvitationLog> InvitationLog { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceReport> DeviceReports { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }


        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(new CancellationToken());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            this.OnBeforeSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaveChanges()
        {
            string action_by_user = this._userResolverService.GetUser();
            DateTimeOffset action_completed = DateTimeOffset.UtcNow;

            var entries = ChangeTracker.Entries().Where(x => x.Entity is ITrackableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                ITrackableEntity entity = (ITrackableEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedByUser = action_by_user;
                    entity.CreationDate = action_completed;
                }
                entity.LastChangeUser = action_by_user;
                entity.LastModifiedOn = action_completed;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region modelBuilder.Ignore
            modelBuilder.Ignore(typeof(AdditionalCharge));
            modelBuilder.Ignore(typeof(CargoItem));
            modelBuilder.Ignore(typeof(ContainerType));
            modelBuilder.Ignore(typeof(CurrencyCountry));
            modelBuilder.Ignore(typeof(DestinationCharge));
            modelBuilder.Ignore(typeof(EquipmentItem));
            modelBuilder.Ignore(typeof(FCLItem));
            modelBuilder.Ignore(typeof(FreightCharge));
            modelBuilder.Ignore(typeof(FreightMovement));
            modelBuilder.Ignore(typeof(FreightMovementItem));
            modelBuilder.Ignore(typeof(ImportProductDimensions));
            modelBuilder.Ignore(typeof(LCLItem));
            modelBuilder.Ignore(typeof(Location));
            modelBuilder.Ignore(typeof(OrganisationNote));
            modelBuilder.Ignore(typeof(OrganisationType));
            modelBuilder.Ignore(typeof(OriginCharge));
            modelBuilder.Ignore(typeof(Partnership));
            modelBuilder.Ignore(typeof(Port));
            modelBuilder.Ignore(typeof(ProductDocument));
            modelBuilder.Ignore(typeof(PurchaseOrder));
            modelBuilder.Ignore(typeof(PurchaseOrderAttachedProductDocument));
            modelBuilder.Ignore(typeof(PurchaseOrderChargeItem));
            modelBuilder.Ignore(typeof(PurchaseOrderDocument));
            modelBuilder.Ignore(typeof(PurchaseOrderEvent));
            modelBuilder.Ignore(typeof(PurchaseOrderItem));
            modelBuilder.Ignore(typeof(PurchaseOrderItemNote));
            modelBuilder.Ignore(typeof(PurchaseOrderItemScheduleLine));
            modelBuilder.Ignore(typeof(PurchaseOrderNote));
            modelBuilder.Ignore(typeof(Quotation));
            modelBuilder.Ignore(typeof(QuotationRequest));
            modelBuilder.Ignore(typeof(Route));
            modelBuilder.Ignore(typeof(Schedule));
            modelBuilder.Ignore(typeof(Shipment));
            modelBuilder.Ignore(typeof(ShipmentDocument));
            modelBuilder.Ignore(typeof(ShipmentEvent));
            modelBuilder.Ignore(typeof(Total));
            #endregion

            modelBuilder.Entity<ApplicationUser>(builder =>
            {
                builder.Property(x => x.GivenName).HasMaxLength(64);
                builder.Property(x => x.Surname).HasMaxLength(64);
                builder.Property(x => x.FullName).HasMaxLength(128);

                builder.Property(x => x.RegistrationDate).HasColumnType("datetime2");

                builder.HasOne(x => x.Organisation).WithMany().HasForeignKey(x => x.OrganisationId);

                builder.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.LocationId);

                builder.Property(x => x.Status).HasConversion(
                    v => v.ToString(),
                    v => (UserStatus)Enum.Parse(typeof(UserStatus), v));
            });

            modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
            modelBuilder.ApplyConfiguration(new InvitationLogConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderLineItemConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceReportConfiguration());

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18, 4)");
            }
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FlareDbContext>
    {
        public FlareDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Flare.API/appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<FlareDbContext>();

            var connectionString = configuration.GetConnectionString("FlareDB");
            builder.UseSqlServer(connectionString);

            return new FlareDbContext(builder.Options, new DesignTimeDbContextUser("test@tradefact.com"));
        }

        protected class DesignTimeDbContextUser : IUserResolverService
        {
            private string _user { get; set; }
            public DesignTimeDbContextUser(string user)
            {
                this._user = user;
            }

            public string GetUser()
            {
                return _user;
            }
        }
    }

}

