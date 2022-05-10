using System;

namespace Core.Models
{
    public class PartnerRate : CosmosItem<PartnerRate>
    {
        public string Country { get; set; }

        public decimal? Customs { get; set; }

        public decimal? Documents { get; set; }

        public decimal? ExportClearanceUsd { get; set; }

        public decimal? ExportThcCost { get; set; }

        public decimal? ExportThcUsd { get; set; }

        public decimal? FT20 { get; set; }

        public decimal? FT40 { get; set; }

        public decimal? FTHHQ40 { get; set; }

        public decimal? FTHQ45 { get; set; }

        public decimal? InboundThc { get; set; }

        public decimal? Insurance { get; set; }

        public string Operator { get; set; }

        public override string PartitionKeyValue => PartnerId.ToString();

        public string PortCode { get; set; }

        public decimal? PortFees { get; set; }

        public string PortOfDischarge { get; set; }

        public string PortOfLoading { get; set; }

        public decimal? RoadCollection { get; set; }

        public decimal? RoadDelivery { get; set; }

        public string Supplier { get; set; }

        public int TransitTime { get; set; }

        public Guid PartnerId { get; set; }
        // public Partner Partner { get; set; }

    }
}