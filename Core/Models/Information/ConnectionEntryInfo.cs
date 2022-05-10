using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ConnectionEntryInfo
    {
        public SourceEnum Source { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephone { get; set; }
        public string Currency { get; set; }
        public int PaymentTerms { get; set; } = 0;
        public int Locations { get; set; } = 0;
        public int ActiveOrders { get; set; } = 0;
        public int ActiveShipments { get; set; } = 0;

        public int Contacts { get; set; } = 0;

        public DateTime CreationDateInternal { get; set; }
        public string TCs { get; set; }
        public string Status { get; set; }
        public Guid ParentId { get; set; }
    }

    public class SupplierInfo: ConnectionEntryInfo
    {
    }

    public class BuyerInfo : ConnectionEntryInfo
    {
    }

    public class ConnectionInfo : ConnectionEntryInfo
    {
    }
}
