using Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Organisation : BaseEntity<Organisation>
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephone { get; set; }
        public int PaymentTerms { get; set; } = 0;
        public string TCs { get; set; }
        public string Currency { get; set; } = "USD";
        public Guid? ParentId { get; set; }
        public BankAccount Bank { get; set; }
        public Organisation Parent { get; set; }

        //public InvoiceAddress InvoiceAddress { get; set; }
        public string TaxId { get; set; }

        public List<Organisation> Directory { get; set; }
        public List<OrganisationNote> Notes { get; set; }
        public List<Address> Addresses { get; set; } = new List<Address>();
        public List<OrganisationContact> Contacts { get; set; } = new List<OrganisationContact>();
        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public OrganisationTypeEnum OrganisationTypeId { get; set; }
        public OrganisationType OrgansiationType { get; set; }
        [JsonIgnore]
        public List<FreightMovement> FreightMovements { get; set; } = new List<FreightMovement>();
        [JsonIgnore]
        public List<Shipment> Shipments { get; set; }

        public int PlanInvitesAvailable { get; set; } = 20;
        public int InvitesIssued { get; set; } = 0;
        public int InvitesActioned { get; set; } = 0;


        [JsonIgnore]
        public List<Partnership> LogisticsProviders { get; set; }
        [JsonIgnore]
        public List<Partnership> LogisticsClients { get; set; }
        public List<Document> Documents { get; set; }

        public List<ProductSupplier> ProductsSupplied { get; set; }

        public string ChargebeeSubscriptionId { get; set; }

        public NetworkConnectionTypeEnum GetNetworkConnectionType(Guid organisation_id)
        {
            if (this.ParentId == organisation_id) return NetworkConnectionTypeEnum.MANAGED;
            if (this.ParentId is null && this.Id != organisation_id) return NetworkConnectionTypeEnum.CONNECTED;

            return NetworkConnectionTypeEnum.SELF;
        }
        public bool? GenericSKUEnabled { get; set; }
    }

    public class OrganisationNote : BaseEntity<OrganisationNote>
    {
        public Guid OrganisationId { get; set; }

        public string Note { get; set; }

        public Organisation Parent { get; set; }
    }

    public class OrganisationType
    {
        public OrganisationTypeEnum OrganisationTypeId { get; set; }
        public string Name { get; set; }
        public List<Organisation> Organisations { get; set; }
    }

    public class DirectoryEntry : Organisation
    {
        public DirectoryEntry()
        {
        }
    }

}