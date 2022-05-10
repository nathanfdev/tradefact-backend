using System;

namespace Core.Models
{
    public class PartnerRateHaulage : BaseEntity<PartnerRateHaulage>
    {
        public string City { get; set; }

        public string Country { get; set; }

        public decimal Gbbfs20Ft { get; set; }

        public decimal Gbbfs40Ft { get; set; }

        public decimal Gfbbs40Ft { get; set; }

        public string ItemType { get; set; }

        public string Postcode { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

    }
}
