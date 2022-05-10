using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tradefact.Api.Common.Configuration;

namespace Tradefact.Api.Common.Extensions
{
    public static class ConfigurationCollectionExtensions
    {
        /// <summary>
        /// Adds A2zureAdfB2C to the IServiceCollection and configure it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.Configure<SwaggerConfiguration>(configuration.GetSection("Tradefact:Settings:Api:Swagger"));
            services.AddSingleton<IValidateOptions<SwaggerConfiguration>, SwaggerConfigurationValidation>();

            services.Configure<AzureAdB2CConfiguration>(configuration.GetSection("Tradefact:Settings:Api:AzureAdB2C"));
            services.AddSingleton<IValidateOptions<AzureAdB2CConfiguration>, AzureAdB2CConfigurationValidation>();

            return services;
        }
    }
}
