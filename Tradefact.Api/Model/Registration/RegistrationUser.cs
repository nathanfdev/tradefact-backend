using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Registration
{
    public class RegistrationUser
    { 
        public string Name { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public bool IsAdmin { get; set; }
    }
}
