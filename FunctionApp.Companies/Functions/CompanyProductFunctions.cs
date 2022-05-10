using Core.Interfaces;
using Core.Models;
using FunctionApp.Companies.Requests;
using Infrastructure.Functions.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;
using System.Threading.Tasks;

namespace FunctionApp.Companies.Functions
{
    public class CompanyProductFunctions
    {
        readonly ICosmosContainerProxy _proxy;

        readonly ICosmosRepository _repository;

        public CompanyProductFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(Product), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(CreateProductRequest))]
        [FunctionName(nameof(CreateProduct))]
        public async Task<IActionResult> CreateProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<CreateProductRequest, CreateProductRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Product>();

            item = await _repository.CreateAsync(item, _proxy.Products).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, null, Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DeactivateProductRequest))]
        [FunctionName(nameof(DeactivateProduct))]
        public async Task<IActionResult> DeactivateProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
    await req.GetJsonBody<DeactivateProductRequest, DeactivateProductRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            string companyId = request.Value.ProductId;
            string productId = request.Value.ProductId;

            await _repository.DeactivateAsync<Product>(productId, companyId, _proxy.Companies).ConfigureAwait(false);

            return new OkResult();
        }

        [SwaggerResponse(200, typeof(Product), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(GetProductRequest))]
        [FunctionName(nameof(GetProduct))]
        public async Task<IActionResult> GetProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
    await req.GetJsonBody<GetProductRequest, GetProductRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            string companyId = request.Value.CompanyId;
            string productId = request.Value.ProductId;

            var product =
                    await _repository.GetAsync<Product>(productId, companyId, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(product);
        }

        [SwaggerResponse(200, typeof(Product[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ListProductsRequest))]
        [FunctionName(nameof(ListProducts))]
        public async Task<IActionResult> ListProducts([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
    await req.GetJsonBody<ListProductsRequest, ListProductsRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            string companyId = request.Value.CompanyId;
            var isActive = request.Value.IsActive;

            var items = await _repository.GetAsync<Product>(c => (c.CompanyId == new System.Guid(companyId)) && (c.IsActive == isActive), _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(items);
        }

        [SwaggerResponse(200, typeof(Product), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdateProductRequest))]
        [FunctionName(nameof(UpdateProduct))]
        public async Task<IActionResult> UpdateProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<UpdateProductRequest, UpdateProductRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var product = request.Value.Adapt<Product>();

            product = await _repository.UpdateAsync(product, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(product);
        }
    }
}