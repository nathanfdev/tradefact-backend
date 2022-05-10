using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tradefact.Api.Common.Configuration;
using Tradefact.Api.Common.Swagger;
using OpenApiInfo = Microsoft.OpenApi.Models.OpenApiInfo;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SwaggerCollectionExtensions
    {
        /// <summary>
        /// Adds Swagger service to the IServiceCollection and configure it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            SwaggerConfiguration swaggerConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<SwaggerConfiguration>>().Value;

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(swaggerConfiguration.Version, new OpenApiInfo { Title = swaggerConfiguration.ApplicationName, Version = swaggerConfiguration.Version });

                options.DocumentFilter<DocumentFilterAddHealth>();

                options.AddSecurityDefinition("Bearer", new OpenApi.Models.OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme",
                });

                options.AddSecurityRequirement(new OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                    },
                    new[] { "readAccess", "writeAccess" }
                },
            });

                options.AddSecurityRequirement(new OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                            },
                            new[] { "readAccess", "writeAccess" }
                        },
                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        /// <summary>
        /// Adds Swagger service to the IServiceCollection and configure it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddNSwag(this IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            SwaggerConfiguration swaggerConfiguration = new SwaggerConfiguration();
            configuration.GetSection("Swagger").Bind(swaggerConfiguration);

            services.AddOpenApiDocument(config =>
                {
                    config.AllowReferencesWithProperties = true;

                    if (swaggerConfiguration.B2CEnabled)
                    {
                        AzureAdB2CConfiguration azureAdConfig = new AzureAdB2CConfiguration();
                        configuration.GetSection("AzureAdB2C").Bind(azureAdConfig);

                        config.AddSecurity("bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
                        {
                            Type = OpenApiSecuritySchemeType.OAuth2,
                            Description = "B2C Authentication",
                            Flow = OpenApiOAuth2Flow.Implicit,
                            Flows = new NSwag.OpenApiOAuthFlows()
                            {
                                Implicit = new NSwag.OpenApiOAuthFlow()
                                {
                                    Scopes = new Dictionary<string, string>
                                {
                                { "openid", "Read access to protected resources" },
                                { $"https://{azureAdConfig.Tenant}/api/dev.read", "Read access to protected resources" },
                                { $"https://{azureAdConfig.Tenant}/api/dev.write", "Write access to protected resources" },
                                },
                                    AuthorizationUrl = $"https://{azureAdConfig.HostName}/{azureAdConfig.Tenant}/{azureAdConfig.Policy}/oauth2/v2.0/authorize",
                                    TokenUrl = $"https://{azureAdConfig.HostName}/{azureAdConfig.Tenant}/{azureAdConfig.Policy}/oauth2/v2.0/token",
                                },
                            },
                        });

                        config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
                    }

                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "Tradefact API";
                        document.Info.Description = "Tradefact API - Next generation customer execution supply chain platform that turns information into action. Tradefact connects the systems, information and people needed to determine, measure, and resolve logistics and related supply chain issues in more real-time, with greater predictability, and with less effort.";
                        document.Info.TermsOfService = "None";
                        document.Info.Contact = new NSwag.OpenApiContact
                        {
                            Name = "Tradefact",
                            Email = swaggerConfiguration.Email,
                            Url = swaggerConfiguration.Url,
                        };
                        document.Info.License = new NSwag.OpenApiLicense
                        {
                            Name = "Use under LICX",
                            Url = $"{swaggerConfiguration.Url}license",
                        };
                    };
                });

            return services;
        }
    }
}
