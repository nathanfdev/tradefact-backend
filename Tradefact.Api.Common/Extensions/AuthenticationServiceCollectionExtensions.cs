using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Tradefact.Api.Common.Configuration;

namespace Tradefact.Api.Common.Extensions
{
    public static class AuthenticationServiceCollectionExtensions
    {
        /// <summary>
        /// Add Azure B2C service to the Service Collection and configure it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureB2C(this IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            AzureAdB2CConfiguration azureAdConfig = new AzureAdB2CConfiguration();
            configuration.GetSection("AzureAdB2C").Bind(azureAdConfig);  //Tradefact:Settings:Api:AzureAdB2C

            services
               .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
               .AddJwtBearer(jwtOptions =>
               {
                    jwtOptions.Authority = $"https://{azureAdConfig.HostName}/tfp/{azureAdConfig.Tenant}/{azureAdConfig.Policy}";
                    jwtOptions.Audience = azureAdConfig.ClientId;

                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                    };

                    jwtOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            context.NoResult();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            string response = JsonConvert.SerializeObject("The access token provided is not valid.");
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                                response =
                                    JsonConvert.SerializeObject("The access token provided has expired.");
                            }

                            context.Response.WriteAsync(response);
                            return Task.CompletedTask;

                        },

                        OnMessageReceived = context =>
                        {
                            // Get the token from some other location
                            // This can also await, if necessary
                            var token = context.Request.Headers["Bearer"];

                            // Set the Token property on the context to pass the token back up to the middleware
                            context.Token = token;

                            return Task.FromResult(true);
                        },

                        OnTokenValidated = context =>
                        {
                            // Check if the user has an OID claim
                            if (!context.Principal.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"))
                            {
                                context.Fail($"The claim 'oid' is not present in the token.");
                            }

                            return Task.CompletedTask;
                        },
                    };
                  });

            return services;
        }

        /// <summary>
        /// Adds A2zureAdfB2C to the IServiceCollection and configure it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationWithAuthorizationSupport(this IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // services.AddMicrosoftIdentityWebApiAuthentication(configuration, "AzureAdB2C", "Bearer");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
#pragma warning disable SA1116 // Split parameters should start on line after declaration
                .AddMicrosoftIdentityWebApi(options => {
                    configuration.Bind("AzureAdB2C", options);
                    options.TokenValidationParameters.NameClaimType = "name";
                },
#pragma warning restore SA1116 // Split parameters should start on line after declaration
                    options => {
                        configuration.Bind("AzureAdB2C", options);
                    });

            return services;
        }
    }

}
