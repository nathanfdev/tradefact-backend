using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class OrganisationRegistrationUser
    {
        public Guid Id { get; set; }
        public Guid OrganisationRegistrationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public bool IsAdmin { get; set; }
    }
}
