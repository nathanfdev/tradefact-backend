using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public long? ExternalProviderId { get; set; }

        public string FullName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }

        public bool IsAdmin { get; set; } = false;
        public UserStatus Status { get; set; }

        public Guid? ProfileImageId { get; set; }
        public Guid? OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
        public Document ProfileImage { get; set; }

        public Guid? LocationId { get; set; }
        public Address Address { get; set; }
        public Guid? ExternalProverUUID { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public Guid? InvitationId { get; set; }
        public string UserPreferences { get; set; }
        public bool IsExposedToOtherOrganization { get; set; }
    }

    public enum UserStatus
    {
        Pending,
        Active,
        Disabled,
        NotFound
    }

}
