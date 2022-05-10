using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Recipient
    {
        public Guid PartnershipId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public RecipientType Type { get; set; }
    }
}
