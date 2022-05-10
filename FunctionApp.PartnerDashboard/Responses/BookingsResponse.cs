using Core.Enums;

namespace FunctionApp.PartnerDashboard.Responses
{
    public class BookingsResponse
    {
        public string CompanyName { get; set; }

        public string ETA { get; set; }

        public string Id { get; set; }

        public string ImportName { get; set; }

        public ImportStatus ImportStatus { get; set; }
    }
}