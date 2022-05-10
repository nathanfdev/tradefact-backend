using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Supplier : BaseEntity<Supplier>
    {
        public Address Address { get; set; }

        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public string ContactTelephone { get; set; }

        public string Name { get; set; }

        public Guid CompanyId { get; set; }

        public Organisation Company { get; set; }

        //public List<Import> Activities { get; set; } = new List<Import>();

    }
}