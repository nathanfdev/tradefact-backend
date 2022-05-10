
using Core.Attributes;
using System;
using Tradefact.Data;

namespace Tradfact.Api.Requests
{
    [TypescriptAutoGeneration]
    public class UserRequest
    {
        public string FullName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string[] Role { get; set; }
        public Guid? LocationId { get; set; }
        public UserStatus Status { get; set; }
        public bool IsExposedToOtherOrganization { get; set; }
    }
}