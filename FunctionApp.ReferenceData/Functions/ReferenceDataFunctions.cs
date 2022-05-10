using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace FunctionApp.ReferenceData.Functions
{
    public class ReferenceDataFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public ReferenceDataFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(CurrencyModel[]), Description = "OK result")]
        [FunctionName(nameof(CurrencyList))]
        public async Task<IActionResult> CurrencyList([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "currencies")]
            HttpRequest req, ILogger logger)
        {

            var query = new QueryDefinition("SELECT * FROM ReferenceData");

            var items = await _repository.GetAsync<CurrencyModel>(query, _proxy.ReferenceData).ConfigureAwait(false);

            return new OkObjectResult(items);
        }
    }
}