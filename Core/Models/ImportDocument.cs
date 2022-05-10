using System;
using Core.Enums;

namespace Core.Models
{
    public class ImportDocument : CosmosItem<ImportDocument>
    {
        public string BlobName { get; set; }

        public string CompanyId { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        public string Extension { get; set; }

        public string ShipmentId { get; set; }

        public string Name { get; set; }

        public OwnershipEnum OwnedBy { get; set; }

        public override string PartitionKeyValue => CompanyId;
    }

    public class ShipmentDocument
    {
        public Guid ShipmentId { get; set; }
        public Guid DocumentId { get; set; }

        public Shipment Shipment { get; set; }
        public Document Document { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class PurchaseOrderAttachedProductDocument
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid PurchaseOrderProductId { get; set; }
        public Guid DocumentId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
        public ProductDocument ProductDocument { get; set; }

        public bool IsActive { get; set; } = true;
    }


}
