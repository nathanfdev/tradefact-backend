using Microsoft.Extensions.Options;

namespace Tradefact.Api.Common.Configuration
{
    public class AzureAdB2CConfigurationValidation : IValidateOptions<AzureAdB2CConfiguration>
    {
        public ValidateOptionsResult Validate(string name, AzureAdB2CConfiguration options)
        {

            if (string.IsNullOrEmpty(options.HostName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.HostName)} configuration parameter for Azure AD B2C");
            }

            if (string.IsNullOrEmpty(options.Tenant))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Tenant)} configuration parameter for Azure AD B2C");
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ClientId)} configuration parameter for Azure AD B2C");
            }

            if (string.IsNullOrEmpty(options.Policy))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Policy)} configuration parameter for Azure AD B2C");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
