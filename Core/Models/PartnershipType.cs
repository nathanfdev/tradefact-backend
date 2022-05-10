using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PartnershipType
    {
        public PartnershipTypeEnum PartnershipTypeId { get; set; }
        public string Name { get; set; }
        public OrganisationTypeEnum ProviderTypeId { get; set; }
        public OrganisationTypeEnum ClientTypeId { get; set; }

        public List<Partnership> Partnerships { get; set; }
    }


}
