using Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Tradefact.UserManagement.Model
{
    public class Registration: TrackableEntity
    {
        public string CompanyName { get; set; }
        public string PersonName { get; set; }
        public string EmailAddress { get; set; }
        public string Subscription { get; set; }
    }

    public class InvitationRequest: Registration
    {
        public InvitationRequest() {}

        public InvitationRequest(Registration registration)
        {
            this.PersonName = registration.PersonName;
            this.EmailAddress = registration.EmailAddress;
            this.CompanyName = registration.CompanyName;
            this.Subscription = registration.Subscription;
        }

        public Partner InvitationPartner { get; set; }

        public InviteType InviteType { get; set; }
        public InvitationSourceEnum InvitationSource { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PartnershipTypeEnum : int
    {
        [EnumMember(Value = "1")] LOGISTICS = 1,
        [EnumMember(Value = "2")] SUPPLY = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InvitationSourceEnum : int
    {
        [EnumMember(Value = "1")] TRADEFACT = 1,
        [EnumMember(Value = "2")] USER = 2
    }

    public class Partner
    {
        public Guid OrganisationId { get; set; }
        public PartnershipTypeEnum PartnerShipType { get; set; }
        public string Name { get; set; }
    }

    public class TrackableEntity
    {
        public Guid Id { get; set; }

        public string CreatedByUser { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }

}
