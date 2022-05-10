using Core.Enums;
using Newtonsoft.Json;
using System;

namespace Tradefact.Api.Model
{
    public class ContactResource
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public string Department { get; set; }
        public bool IsDefault { get; set; }
        public string Notes { get; set; }
        public Guid? LocationId { get; set; }
        public string LocationName { get; set; }
        public EmailAddressResource Email { get; set; }
        public PhoneResource Phone { get; set; }
        public virtual NetworkConnectionTypeEnum NetworkConnectionType { get; }
    }

    public class OwnedContactResource : ContactResource
    {
        public override NetworkConnectionTypeEnum NetworkConnectionType => NetworkConnectionTypeEnum.SELF;
    }
    public class ManagedContactResource : ContactResource
    {
        public override NetworkConnectionTypeEnum NetworkConnectionType => NetworkConnectionTypeEnum.MANAGED;
    }
    public class ConnectedContactResource : ContactResource
    {
        public override NetworkConnectionTypeEnum NetworkConnectionType => NetworkConnectionTypeEnum.CONNECTED;
    }

    public class EmailAddressResource
    {
        public string Email { get; set; }
        public bool IsDefault { get; set; }

        [JsonProperty(Order = -900)]
        public Guid Id { get; set; }
    }

    public class PhoneResource
    {
        public string CountryCode { get; set; }
        public string Number { get; set; }
        public string AreaCode { get; set; }
        public bool IsDefault { get; set; }

        [JsonProperty(Order = -900)]
        public Guid Id { get; set; }
    }

}
