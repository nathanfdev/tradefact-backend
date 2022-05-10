namespace Tradefact.Api.Common.Configuration
{
    public class AzureAdB2CConfiguration
    {
        public string HostName { get; set; }

        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string Policy { get; set; }

        public string ScopeRead { get; set; }

        public string ScopeWrite { get; set; }
    }
}
