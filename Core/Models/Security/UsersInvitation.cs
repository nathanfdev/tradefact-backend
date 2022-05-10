using Core.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Security
{
    public partial class UsersInvitation : ValueObject
    {
        public string Message { get; set; }
        public IList<string> Roles { get; set; }
        [Required]
        public IList<string> Emails { get; set; }
    }
}
