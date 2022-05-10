using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.Models
{
    public class Import : CosmosItem<Import>
    {
        public DateTime? CollectedDate { get; set; }

        public ImportConsignmentDetails ConsignmentDetails { get; set; }

        public string ContainerNumber { get; set; }

        public string CustomsStatus { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public DateTime? ETA { get; set; }

        public bool InvoicePaid { get; set; }

        public string Latitude { get; set; }

        public LoadTypeEnum LoadType { get; set; }

        public string Longitude { get; set; }

        public string Name { get; set; }

        public override string PartitionKeyValue => CompanyId.ToString();

        public string PartnerId { get; set; }

        public string PoNumber { get; set; }

        public List<string> PurchaseOrders { get; set; }

        public List<ImportQuotation> Quotations { get; set; }

        public int? ShippingProviderId { get; set; }

        public ImportStatus Status { get; set; }

        public string VesselName { get; set; }

        //public Supplier Supplier { get; set; }
        //public Guid SupplierId { get; set; }

        public Organisation Company { get; set; }
        public Guid CompanyId { get; set; }
    }
}