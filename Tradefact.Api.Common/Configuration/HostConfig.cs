namespace Tradefact.Api.Common.Configuration
{
    public class HostConfig
    {
        public SwaggerConfiguration Swagger { get; set; }

        public AzureAdB2CConfiguration AzureB2C { get; set; }

        public SerilogConfig Serilog { get; set; }

        public ApplicationInsightsConfig ApplicationInsights { get; set; }
    }
}
