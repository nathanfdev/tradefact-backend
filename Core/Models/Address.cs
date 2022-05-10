using Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Address : BaseEntity<Address>
    {
        public Guid OrganisationId { get; set; }
        public AddressType Type { get; set; }

        public string Name { get; set; }
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string City { get; set; }

        public string Province { get; set; }
        public string County { get; set; }

        public bool IsDefault { get; set; }
        public bool IsInvoiceAddress { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public Country Country { get; set; }

        public GeographicPosition Position { get; set; }
        public Guid? B2BConnectionId { get; set; }

        //public string CountryCode => Country ??= Country.Code2 | null;
        //public string CountryName => Country.Name;

        [JsonIgnore]
        public string StreetAddress
        {
            get
            {
                return string.Join(", ", AddressLine1, AddressLine2, PostalCode, Country).Trim(',', ' ');
            }
        }

        public override string ToString()
        {
            var retVal = string.Join(" ", Name, StreetAddress);
            return retVal;
        }
        public Organisation Organisation { get; set; }

    }

}
