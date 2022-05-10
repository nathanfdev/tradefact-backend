using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Security
{
    public partial class OrganizationUserRegistration : UserRegistration
    {
        [Required]
        public string Role { get; set; }
        [Required]
        public Guid OrganizationId { get; set; }
    }
}
