namespace Tradefact.Api.Common.Configuration
{

    public class SwaggerConfiguration
    {
        public string Version { get; set; }

        public string ApplicationName { get; set; }

        public string XmlFile { get; set; }

        public string ClientId { get; set; }

        public string Url { get; set; }

        public string Email { get; set; }

        public bool B2CEnabled { get; set; } = true;
    }
}
