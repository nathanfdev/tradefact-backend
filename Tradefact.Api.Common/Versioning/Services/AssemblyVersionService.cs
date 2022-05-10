using Tradefact.Api.Common.Versioning.Interfaces;

namespace Tradefact.Api.Common.Versioning.Services
{
    public class AssemblyVersionService : IVersionService
    {
        public Version GetVersion()
        {
            var buildVersion = typeof(VersionController).Assembly.GetName().Version.ToString();
            return new Version(buildVersion);
        }
    }
}
