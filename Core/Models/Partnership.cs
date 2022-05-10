using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Partnership : BaseEntity<Partnership>
    {
        public Guid ProviderId { get; set; }
        public Guid ClientId { get; set; }

        public Organisation Provider { get; set; }
        public Organisation Client { get; set; }

        public List<Shipment> Shipments { get; set; }

        public PartnershipTypeEnum PartnershipTypeId { get; set; }
        public PartnershipType PartnershipType { get; set; }
    }

}
