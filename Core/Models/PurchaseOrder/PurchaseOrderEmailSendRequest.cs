using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class PurchaseOrderEmailSendRequest
    {
        public List<PurchaseOrderRecipients> Recipients { get; set; }
        public Guid PurchaseOrderId { get; set; }
    }

    public class PurchaseOrderRecipients
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string RegistrationLink { get; set; }
    }

    public class POContactRequest
    {
        public Guid Id { get; set; }

        public string RegistrationLink { get; set; }
    }
}
