using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NSwag.AspNetCore;
using Tradefact.Api.Common.Configuration;

namespace Tradefact.Api.Common.Extensions
{
    public static class TradefactBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, SwaggerConfiguration swaggerConfig)
        {
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } });
            });

            return app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + swaggerConfig.Version + "/swagger.json", swaggerConfig.ApplicationName);
            });
        }

        public static IApplicationBuilder UseNSwag(this IApplicationBuilder app)
        {
            app.UseOpenApi();

            app.UseReDoc(options =>
            {
                options.Path = "/openapi_redoc";
                options.DocumentPath = "/swagger/v1/swagger.json";
            });

            return app.UseSwaggerUi3(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "f1ca1da6-b44c-4ed9-958d-00a98ceda7f5", //swaggerConfig.Value.ClientId,
                    AppName = "Tradefact.Api", //swaggerConfig.Value.ApplicationName,
                };
                // Add multiple OpenAPI/Swagger documents to the Swagger UI 3 web frontend
                settings.SwaggerRoutes.Add(new SwaggerUi3Route("Swagger", "/swagger/v1/swagger.json"));
                settings.SwaggerRoutes.Add(new SwaggerUi3Route("Openapi", "/openapi/v1/openapi.json"));
                settings.TagsSorter = "aplha";
                settings.OperationsSorter = "alpha";
            });
        }
    }
}
