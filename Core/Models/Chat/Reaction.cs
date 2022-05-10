using System.Collections.Generic;

namespace Core.Models
{
    public class Reaction
    {
        public string Name { get; set; }

        public IList<string> Usernames { get; set; }
    }
}