using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class LogisticsClient
    {
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public Guid PartnershipId { get; set; }
        public string InviteId { get; set; }
        public int ActiveShipments { get; set; }
        public int InvitedUsers { get; set; }
        public int ConfirmedUsers { get; set; }
        public DateTime LastFreightMovementEntered { get; set; }
        public DateTime LastShipmentBooked { get; set; }
        public DateTime CreationDateInternal { get; set; }
        public string ContactEmail { get; set; }
        public List<OrganisationCountryResource> CountryInfo { get; set; }
    }

    public class LogisticsClientCountryResource : OrganisationCountryResource
    {
        public Guid OrganisationId { get; set; }
    }
}
