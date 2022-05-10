using System;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class SettingService : ISettingService
    {
        private const string ApiApplicationIdKey = "ApiApplicationId";

        private const string ApiScopeNameKey = "ApiScopeName";

        // B2C
        private const string AuthorityUrlKey = "AuthorityUrl";
        private const string GraphClientId = "GraphClientId";
        private const string GraphClientSecret = "GraphClientSecret";

        // graph settings
        private const string GraphTenantId = "GraphTenantId";

        public string GetApiApplicationId()
        {
            return GetEnvironmentVariable(ApiApplicationIdKey);
        }

        public string GetApiScopeName()
        {
            return GetEnvironmentVariable(ApiScopeNameKey);
        }

        // B2C
        public string GetAuthorityUrl()
        {
            return GetEnvironmentVariable(AuthorityUrlKey);
        }

        public string GetGraphClientId()
        {
            return GetEnvironmentVariable(GraphClientId);
        }

        public string GetGraphClientSecret()
        {
            return GetEnvironmentVariable(GraphClientSecret);
        }

        // graph
        public string GetGraphTenantId()
        {
            return GetEnvironmentVariable(GraphTenantId);
        }


        public string GetSiteName()
        {
            return GetEnvironmentVariable("WEBSITE_SITE_NAME");
        }

        //*** PRIVATE ***//
        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name,
                EnvironmentVariableTarget.Process);
        }
    }
}