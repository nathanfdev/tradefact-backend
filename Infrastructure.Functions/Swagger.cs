using Infrastructure.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag;
using NSwag.SwaggerGeneration.AzureFunctionsV2;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Functions
{
    public static class Swagger
    {
        //[OpenApiIgnore]
        [FunctionName("Swagger")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/json")]
            HttpRequest req, ILogger log)
        {
            var title = Assembly.GetExecutingAssembly().GetName().Name.Replace(".", " ");

            var settings = new AzureFunctionsV2ToSwaggerGeneratorSettings { Title = title, RoutePrefix = string.Empty };

            settings.SerializerSettings = new JsonSerializerSettings { ContractResolver = new OrderedContractResolver() };

            var generator = new AzureFunctionsV2ToSwaggerGenerator(settings);
            var types = AzureFunctionsV2ToSwaggerGenerator.GetAzureFunctionClasses(Assembly.GetExecutingAssembly());
            var document =
                await generator.GenerateForAzureFunctionClassesAsync(types, null).ConfigureAwait(false);
            document.Host = "#{apihostprefix}#.api.tradefact.com";
            document.Schemes.Add(OpenApiSchema.Https);
            var json = document.ToJson();

            return new OkObjectResult(json);
        }
    }
}
