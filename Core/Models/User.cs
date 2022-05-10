using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;

namespace Core.Models
{
    public class User
    {
        public string Avatar { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
    }
}