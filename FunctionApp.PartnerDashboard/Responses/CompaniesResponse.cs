using Core.Models;

namespace FunctionApp.PartnerDashboard.Responses
{
    public class CompaniesResponse
    {
        public Address Address { get; set; }

        public string CompanyName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public string DateRegistered { get; set; }

        public string Id { get; set; }

        public int PaymentTerms { get; set; }
    }
}