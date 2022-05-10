using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class B2BConnection : BaseEntity<B2BConnection>
    {
        public Guid OrgansationId { get; set; }
        public Guid LinkedOrganisationId { get; set; }
        public string Tags { get; set; }
        public decimal Rating { get; set; }

        public List<ConnectionContact> Contacts { get; set; } = new List<ConnectionContact>();

        public Organisation Organisation { get; set; }
        public Organisation LinkedOrganisation { get; set; }

        public ConnectionStatusEnum ConnectionStatus { get; set; }
    }
}
