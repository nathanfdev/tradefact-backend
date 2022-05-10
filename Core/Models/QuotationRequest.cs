using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class QuotationRequest : BaseEntity<QuotationRequest>
    {
        public Guid PartnershipId { get; set; }
        public Guid FreightMovementId { get; set; }
        public Partnership Partnership { get; set; }
        public FreightMovement FreightMovement { get; set; }
        public DateTime Submitted { get; set; }
        public DateTime Actioned { get; set; }
        public QuotationStateEnum State { get; set; }
        public Quotation Quotation => this.Quotations.FirstOrDefault(q=>q.IsActive);
        public List<Quotation> Quotations { get; set; } = new List<Quotation>();
        public List<Shipment> Shipments { get; set; }
    }
}
