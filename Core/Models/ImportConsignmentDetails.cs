using Core.Enums;

namespace Core.Models
{
    public class ImportConsignmentDetails: CosmosItem<ImportConsignmentDetails>
    {
        public LoadEnum ContainerRequirement { get; set; }

        public string DispatchMethod { get; set; }

        public decimal Insurance { get; set; }

        public string PortOfDischarge { get; set; }

        public string PortOfLoading { get; set; }

        public string ShipmentType { get; set; }
    }
}
