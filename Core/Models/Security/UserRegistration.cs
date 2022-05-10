using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models.Security
{
    public partial class UserRegistration
    {
        public string Email { get; set; }
        public string Salutation { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string TimeZone { get; set; }
    }
}
