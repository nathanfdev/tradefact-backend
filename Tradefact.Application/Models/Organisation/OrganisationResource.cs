using Core.Attributes;
using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tradefact.Application.Models
{
    public class OrganisationResource
    {

        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephone { get; set; }
        public string Currency { get; set; }
        public int PaymentTerms { get; set; } = 0;
        public string TaxId { get; set; }
        public string TCs { get; set; }

        public AddressResource InvoiceAddress { get; set; }
    }

    [TypescriptAutoGeneration]
    public class DirectoryResource
    {
        public SourceEnum Source { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephone { get; set; }
        public string Currency { get; set; }
        public int PaymentTerms { get; set; } = 0;
        public int AddressCount { get; set; } = 0;
        public int ActiveOrders { get; set; } = 0;
        public int ActiveShipments { get; set; } = 0;

        public int ContactCount { get; set; } = 0;

        public string Status { get; set; }
        public DateTime CreationDateInternal { get; set; }
        public string TCs { get; set; }

        public NetworkConnectionTypeEnum NetworkType { get; set; }
        public List<OrganisationCountryResource> CountryInfo { get; set; }
    }

    public class SortedDirectoryResource : DirectoryResource
    {
        [JsonIgnore]
        public string Search => $"{this.Name.ToLower()}";
        public int Position(string search) => this.Search.IndexOf(search.ToLower());
    }

    public class DirectoryCountryResource : OrganisationCountryResource
    {
        public Guid OrganisationId { get; set; }
    }

    public class PartnershipResource
    {
        public Guid PartnershipId { get; set; }

        public OrganisationResource Provider { get; set; }
        public OrganisationResource Client { get; set; }
    }

}
