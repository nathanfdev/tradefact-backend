using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public abstract class Contact : BaseEntity<Contact>
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public string Department { get; set; }
        public bool IsDefault { get; set; }
        public Guid LocationId { get; set; }
        public string Notes { get; set; }
    }

    public class OrganisationContact : Contact
    {
        public Guid OrganisationId { get; set; }

        public List<OrganisationContactEmailAddress> Email { get; set; } = new List<OrganisationContactEmailAddress>();
        public List<OrganisationContactPhone> Phone { get; set; } = new List<OrganisationContactPhone>();

    }
    public class ConnectionContact : Contact
    {
        public Guid ConnectionId { get; set; }
        public List<ConnectionContactEmailAddress> Email { get; set; } = new List<ConnectionContactEmailAddress>();
        public List<ConnectionContactPhone> Phone { get; set; } = new List<ConnectionContactPhone>();
    }


    public class EmailAddress : BaseEntity<EmailAddress>
    {
        public Guid ContactId { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; }
    }

    public class OrganisationContactEmailAddress : EmailAddress
    {
        public Guid OrganisationId { get; set; }
    }
    public class ConnectionContactEmailAddress : EmailAddress
    {
        public Guid ConnectionId { get; set; }
    }


    public class Phone : BaseEntity<Phone>
    {
        public Guid ContactId { get; set; }
        public string CountryCode { get; set; }
        public string Number { get; set; }
        public string AreaCode { get; set; }
        public bool IsDefault { get; set; }

    }
    public class OrganisationContactPhone : Phone
    {
        public Guid OrganisationId { get; set; }
    }
    public class ConnectionContactPhone : Phone
    {
        public Guid ConnectionId { get; set; }
    }



    public class ContactInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }


}
