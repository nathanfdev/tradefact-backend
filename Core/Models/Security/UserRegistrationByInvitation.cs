using System.ComponentModel.DataAnnotations;

namespace Core.Models.Security
{
    public partial class UserRegistrationByInvitation : UserRegistration
    {
        [Required]
        public string Token { get; set; }
        public string OrganizationId { get; set; }
    }
}
