using Microsoft.Extensions.Options;

namespace Tradefact.Api.Common.Configuration
{
    public class SwaggerConfigurationValidation : IValidateOptions<SwaggerConfiguration>
    {
        public ValidateOptionsResult Validate(string name, SwaggerConfiguration options)
        {

            if (string.IsNullOrEmpty(options.Version))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Version)} configuration parameter for Swagger");
            }

            if (string.IsNullOrEmpty(options.ApplicationName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ApplicationName)} configuration parameter for Swagger");
            }

            if (string.IsNullOrEmpty(options.XmlFile))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.XmlFile)} configuration parameter for Swagger");
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ClientId)} configuration parameter for Swagger");
            }

            if (string.IsNullOrEmpty(options.Email))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Email)} configuration parameter for Swagger");
            }

            if (string.IsNullOrEmpty(options.Url))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Url)} configuration parameter for Swagger");
            }


            return ValidateOptionsResult.Success;
        }
    }
}
