using Core.Enums;

namespace FunctionApp.PartnerDashboard.Responses
{
    public class BillingResponse
    {
        public string Amount { get; set; }

        public BillingStatus BillingStatus { get; set; }

        public string DateDue { get; set; }

        public string DateIssued { get; set; }

        public string Id { get; set; }

        public string ImportName { get; set; }

        public string ImportType { get; set; }

        public string Reference { get; set; }

        public decimal Tax { get; set; }

        public decimal TotalPrice { get; set; }
    }
}