using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class OrganisationRegistration
    {
        public Guid Id { get; set; }
        public string CompanyLegalName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string WebAddress { get; set; }
        public string BusinessId { get; set; }
        public string BusinessRegDocumentUrl { get; set; }
        public string BusinessRegDocumentName { get; set; }
        public string CompanyBio { get; set; }
        public Guid? OrganisationId { get; set; }
        public OrganisationRegistrationStatusEnum RegistrationStatus { get; set; }
        public List<OrganisationRegistrationUser> RegistrationUsers { get; set; }
    }
}
