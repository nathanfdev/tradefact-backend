using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Obsolete]
    public class Partner : BaseEntity<Partner>
    {
        public string AccountNumber { get; set; }

        public string BankName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public string ContactTelephone { get; set; }
        public Address Address { get; set; }

        public string Currency { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomsFee { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DocumentsFee { get; set; } = 0;

        public string Name { get; set; }

        public string PartnerCode { get; set; }

        public string SortCode { get; set; }

        public string SwiftCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxRate { get; set; }

        public string TosDocumentId { get; set; }

        public string Website { get; set; }

        // public List<Company> Companies { get; set; } = new List<Company>();

        public List<Shipment> Shipments { get; set; }
        // public List<PartnerRate> Rates { get; set; } = new List<PartnerRate>();
        // public List<PartnerRateHaulage> PartnerRateHaulages { get; set; } = new List<PartnerRateHaulage>();

        // public List<Address> Addresses { get; set; } = new List<Address>();

    }

}