using Core.Interfaces;
using Core.Models;
using FunctionApp.Imports.Requests;
using Infrastructure.Functions.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;
using System.Threading.Tasks;

namespace FunctionApp.Imports.Functions
{
    public class ImportProductFunctions
    {
        readonly ICosmosContainerProxy _proxy;

        readonly ICosmosRepository _repository;

        public ImportProductFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(ImportProduct), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(AssignProductRequest))]
        [FunctionName(nameof(AssignProduct))]
        public async Task<IActionResult> AssignProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<AssignProductRequest, AssignProductRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<ImportProduct>();

            item = await _repository.CreateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(ImportProduct[]), Description = "OK result")]
        [FunctionName(nameof(ListProducts))]
        public async Task<IActionResult> ListProducts([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<ListProductsRequest, ListProductsRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var importId = request.Value.ImportId;
            var companyId = request.Value.CompanyId;

            var query =
                     new QueryDefinition(@"SELECT * FROM importProducts i WHERE i.type = ""ImportProduct"" AND i.companyId = @companyId AND i.importId = @importId AND i.isActive=true")
                        .WithParameter("@companyId", companyId)
                .WithParameter("@importId", importId);

            var items = await _repository.GetAsync<ImportProduct>(query, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(items);
        }

        [SwaggerResponse(200, null, Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UnsassignProductRequest))]
        [FunctionName(nameof(UnassignProduct))]
        public async Task<IActionResult> UnassignProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]

            HttpRequest req, ILogger logger)
        {
            var request =
    await req.GetJsonBody<UnsassignProductRequest, UnassignProductRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var productId = request.Value.ProductId;

            await _repository.DestroyAsync<ImportProduct>(productId, companyId, _proxy.Imports).ConfigureAwait(false);

            return new OkResult();
        }

        [SwaggerResponse(200, typeof(ImportProduct), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdateProductQuantityRequest))]
        [FunctionName(nameof(UpdateProductQuantity))]
        public async Task<IActionResult> UpdateProductQuantity([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]

            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<UpdateProductQuantityRequest, UpdateProductQuantityRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var productId = request.Value.ProductId;
            var quantity = request.Value.Quantity;

            var item = await _repository.GetAsync<ImportProduct>(productId, companyId, _proxy.Imports).ConfigureAwait(false);
            item.Quantity = quantity;

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}