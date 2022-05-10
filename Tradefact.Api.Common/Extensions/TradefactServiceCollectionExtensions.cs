using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSwag;
using NSwag.Generation.Processors.Security;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tradefact.Api.Common.Configuration;
using Tradefact.Api.Common.Swagger;
using OpenApiInfo = Microsoft.OpenApi.Models.OpenApiInfo;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TradefactServiceCollectionExtensions
    {

        /// <summary>
        /// Adds MVC service to the Service Collection and configure json serializer behavior
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IMvcBuilder AddMvcService(this IServiceCollection services) =>
            services.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        /// <summary>
        /// Add Azure B2C service to the Service Collection and configure it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="azureAdConfig"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAzureB2C(this IServiceCollection services, AzureAdB2CConfiguration azureAdConfig) =>
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(jwtOptions =>
              {
                  jwtOptions.Authority = $"https://{azureAdConfig.HostName}/tfp/{azureAdConfig.Tenant}/{azureAdConfig.Policy}";
                  jwtOptions.Audience = azureAdConfig.ClientId;

                  jwtOptions.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateAudience = false,
                      ValidateIssuer = false
                  };

                  // jwtOptions.TokenValidationParameters.ValidateIssuer = false;

                  jwtOptions.Events = new JwtBearerEvents
                  {
                      OnAuthenticationFailed = context =>
                      {
                          var s = $"AuthenticationFailed: {context.Exception.Message}";
                          context.Response.ContentLength = s.Length;
                          context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(s), 0, s.Length);

                          return Task.FromResult(true);
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

                          ////Get the calling app client id that came from the token produced by Azure AD
                          // string userId = context.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                          // var claims = new List<Claim>{
                          //          new Claim("CustomerId", "A1B2C3")
                          //      };
                          // var appIdentity = new ClaimsIdentity(claims);
                          // context.Principal.AddIdentity(appIdentity);
                          // ClaimsPrincipal userPrincipal = context.Principal;
                          return Task.CompletedTask;
                      },
                  };
              });

        /// <summary>
        /// Add AutoMapper service to the Service Collection and configure it assemblies
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblyNamesToScan"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, IEnumerable<string> assemblyNamesToScan)
        {
            services.TryAddSingleton(GenerateMapperConfiguration(assemblyNamesToScan));
            return services;
        }

        private static IMapper GenerateMapperConfiguration(IEnumerable<string> assemblyNamesToScan)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(assemblyNamesToScan);
            });
            return config.CreateMapper();
        }

    }
}
