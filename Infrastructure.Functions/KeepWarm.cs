using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Infrastructure.Functions
{
    public static class KeepWarm
    {
        [OpenApiIgnore]
        [FunctionName("KeepWarm")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "")]
            HttpRequest req, ILogger log) => await Task.FromResult(new OkResult());
    }
}
