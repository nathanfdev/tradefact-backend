using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Models.External;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Data.Configuration;

namespace Tradefact.Data
{
    public class TradefactDbContext : IdentityDbContext<ApplicationUser>
    {
        // private const string connection_string = "Server=tcp:sql-tradefact-dev.database.windows.net,1433;Initial Catalog=sqldb-tradefact-dev;Persist Security Info=False;User ID=sql-admin-dev;Password=Tr4dE73ChurchF4ct;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly IUserResolverService _userResolverService;

        public TradefactDbContext(DbContextOptions<TradefactDbContext> options, IUserResolverService userResolverService)
            : base(options)
        { 
            this._userResolverService = userResolverService;
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        //public static TradefactDbContext Create(IUserResolverService userResolverService)
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<TradefactDbContext>();
        //    optionsBuilder.UseSqlServer(connection_string);

        //    TradefactDbContext retvalue =  new TradefactDbContext(optionsBuilder.Options, userResolverService);
        //    retvalue.ChangeTracker.LazyLoadingEnabled = false;

        //    return retvalue;
        //}

        //public DbSet<Partner> Partners { get; set; }
        public DbSet<OrganisationType> OrganisationTypes { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Partnership> Partnerships { get; set; }
        public DbSet<PartnershipType> PartnershipTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDocument> ProductDocuments { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Activity> Activities { get; set; }

        public DbSet<ProductSupplier> ProductSuppliers { get; set; }
        public DbSet<ProductSupplierCurrency> ProductSupplierCurrency { get; set; }

        public DbSet<FreightMovement> FreightMovements { get; set; }
        public DbSet<FreightMovementItem> FreightMovementItems { get; set; }
        public DbSet<CargoItem> CargoItems { get; set; }

        public DbSet<QuotationRequest> QuotationRequests { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<FreightCharge> FreightCharges { get; set; }
        public DbSet<OriginCharge> OriginCharges { get; set; }
        public DbSet<DestinationCharge> DestinationCharges { get; set; }
        public DbSet<AdditionalCharge> AdditionalCharges { get; set; }
        public DbSet<QuotationChargeType> QuotationChargeTypes { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PurchaseOrderChargeItem> PurchaseOrderChargeItems { get; set; }
        public DbSet<PurchaseOrderDocument> PurchaseOrderDocuments { get; set; }
        public DbSet<PurchaseOrderAttachedProductDocument> PurchaseOrderAttachedProductDocuments { get; set; }

        public DbSet<PurchaseOrderItemScheduleLine> PurchaseOrderItemScheduleLines { get; set; }
        public DbSet<PurchaseOrderEvent> PurchaseOrderEvents { get; set; }

        public DbSet<PurchaseType> PurchaseType { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentDocument> ShipmentDocuments { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ShipmentType> ShipmentTypes { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<Airline> AirLines { get; set; }
        public DbSet<ContainerType> ContainerTypes { get; set; }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<BuyerInfo> Buyers { get; set; }
        public DbSet<SupplierInfo> Suppliers { get; set; }

        public DbSet<ConnectionInfo> Connections { get; set; }

        public DbSet<InvitationLog> InvitationLog { get; set; }
        public DbSet<QueuedTask> QueuedTasks { get; set; }

        public DbSet<OrganisationRegistration> OrganisationRegistrations { get; set; }

        public DbSet<B2BConnection> B2BConnections { get; set; }

        public DbSet<ConnectionContact> B2BConnectionContacts { get; set; }
        public DbSet<NewsFeed> NewsFeeds { get; set; }

        public DbSet<ExternalPurchaseOrder> ExternalPurchaseOrders { get; set; }
        public DbSet<ExternalPurchaseOrderLineItem> ExternalPurchaseOrderLineItems { get; set; }
        public DbSet<ExternalProduct> ExternalProducts { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

        //public DbSet<PartnerRate> PartnerRates { get; set; }
        //public DbSet<PartnerRateHaulage> PartnerHaulageRates { get; set; }
        // public DbSet<User> Users { get; set; }

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

        public void UpdateTrackProperties<T>(T item, bool isDelete)
            where T : CosmosItem<T>
        {
            item.IsActive = isDelete;
            item.UpsertDate = DateTime.UtcNow;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, CustomAnnotationProvider>();
        //    optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomSqlServerMigrationsSqlGenerator>();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(builder =>
            {
                builder.Property(x => x.GivenName).HasMaxLength(64);
                builder.Property(x => x.Surname).HasMaxLength(64);
                builder.Property(x => x.FullName).HasMaxLength(128);
                
                builder.Property(x => x.RegistrationDate).HasColumnType("datetime2");

                // Each User can have an optional profile picture
                builder.HasOne(x => x.Organisation).WithMany().HasForeignKey(x => x.OrganisationId);
                builder.HasOne(x => x.ProfileImage).WithMany().HasForeignKey(x => x.ProfileImageId);

                builder.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.LocationId);

                builder.Property(x => x.Status).HasConversion(
                    v => v.ToString(),
                    v => (UserStatus)Enum.Parse(typeof(UserStatus), v));
            });

            //Application Configurations
            // modelBuilder.ApplyConfiguration(new PartnerConfiguration());
            modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());

            modelBuilder.ApplyConfiguration(new PartnershipConfiguration());
            modelBuilder.ApplyConfiguration(new B2BConnectionConfiguration());

            modelBuilder.ApplyConfiguration(new FreightMovementConfiguration());
            modelBuilder.ApplyConfiguration(new FreightMovementItemConfiguration());
            modelBuilder.ApplyConfiguration(new CargoItemConfiguration());


            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductDocumentConfiguration());

            modelBuilder.ApplyConfiguration(new ProductVariantConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSupplierConfiguration());

            modelBuilder.ApplyConfiguration(new OrganisationTypeConfiguration());

            modelBuilder.ApplyConfiguration(new QuotationRequestConfiguration());

            modelBuilder.ApplyConfiguration(new QuotationConfiguration());
            modelBuilder.ApplyConfiguration(new QuotationChargeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QuotationChargeItemConfiguration());

            // Purchase Order
            modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderItemConfiguration());

            // Purchase Order Notes
            modelBuilder.ApplyConfiguration(new PurchaseOrderNoteConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderItemNoteConfiguration());

            modelBuilder.ApplyConfiguration(new PurchaseOrderChargeItemConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderAttachedProductDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderItemScheduleLineConfiguration());

            modelBuilder.ApplyConfiguration(new PurchaseOrderEventConfiguration());

            modelBuilder.ApplyConfiguration(new ShipmentTypeConfiguration());

            modelBuilder.ApplyConfiguration(new AirlineConfiguration());
            modelBuilder.ApplyConfiguration(new CarrierConfiguration());

            modelBuilder.ApplyConfiguration(new ContainerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyCountryConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());

            modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
            modelBuilder.ApplyConfiguration(new ShipmentDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new ShipmentEventConfiguration());

            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());

            modelBuilder.ApplyConfiguration(new InvitationLogConfiguration());

            modelBuilder.ApplyConfiguration(new OrganisationRegistrationConfiguration());

            modelBuilder.ApplyConfiguration(new QueuedTaskConfiguration());

            modelBuilder.ApplyConfiguration(new NewsFeedConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalPurchaseOrderConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalPurchaseOrderLineItemConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalProductConfiguration());
            modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());

            modelBuilder.Entity<Carrier>().HasKey(p => p.SCAC);

            modelBuilder.Entity<BuyerInfo>().HasNoKey().ToView("vwBuyers"); 

            modelBuilder.Entity<SupplierInfo>().HasNoKey().ToView("vwSuppliers"); 

            modelBuilder.Entity<ConnectionInfo>().HasNoKey().ToView("vwConnections");

            // modelBuilder.Entity<UserProfile>().HasNoKey().ToView("vwUserProfiles");

            //foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18, 4)");
            }

            modelBuilder.Seed();
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganisationType>().HasData(
                new OrganisationType
                {
                    OrganisationTypeId = OrganisationTypeEnum.SHIPPER,
                    Name = "SHIPPER"
                },
                new OrganisationType
                {
                    OrganisationTypeId = OrganisationTypeEnum.SUPPLIER,
                    Name = "SUPPLIER"
                }, new OrganisationType
                {
                    OrganisationTypeId = OrganisationTypeEnum.BUYER,
                    Name = "BUYER"
                }
            );

            modelBuilder.Entity<QuotationChargeType>().HasData(
                new QuotationChargeType
                {
                    QuotationChargeTypeId = QuotationChargeTypeEnum.FREIGHT,
                    Name = "FREIGHT"
                },
                new QuotationChargeType
                {
                    QuotationChargeTypeId = QuotationChargeTypeEnum.ORIGIN,
                    Name = "ORIGIN"
                },
                new QuotationChargeType
                {
                    QuotationChargeTypeId = QuotationChargeTypeEnum.DESTINATION,
                    Name = "DESTINATION"
                },
                new QuotationChargeType
                {
                    QuotationChargeTypeId = QuotationChargeTypeEnum.ADDITIONAL,
                    Name = "ADDITIONAL"
                }
            );

            modelBuilder.Entity<ShipmentType>().HasData(
                new ShipmentType
                {
                    Id = ShipmentTypeEnum.AIR,
                    Name = "AIR"
                },
               new ShipmentType
               {
                   Id = ShipmentTypeEnum.SEA,
                   Name = "SEA"
               }
            );
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TradefactDbContext>
    {
        public TradefactDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Tradefact.API/appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TradefactDbContext>();

            var connectionString = configuration.GetConnectionString("TradefactDB");
            builder.UseSqlServer(connectionString);

            return new TradefactDbContext(builder.Options, new DesignTimeDbContextUser("test@tradefact.com"));
        }

        protected class DesignTimeDbContextUser: IUserResolverService
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
