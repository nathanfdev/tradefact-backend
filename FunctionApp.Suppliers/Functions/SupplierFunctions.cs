using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using FunctionApp.Suppliers.Requests;
using Infrastructure.Functions.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Suppliers.Functions
{
    public class SupplierFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public SupplierFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, null, Description = "OK result")]
        [FunctionName(nameof(DeactivateSupplier))]
        public async Task<IActionResult> DeactivateSupplier([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<DeactivateSupplierRequest, DeactivateSupplierRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var supplierId = request.Value.SupplierId;

            await _repository.DeactivateAsync<Supplier>(supplierId, supplierId, _proxy.Suppliers).ConfigureAwait(false);

            return new OkResult();
        }

        [SwaggerResponse(200, typeof(Supplier), Description = "OK result")]
        [FunctionName(nameof(GetSupplier))]
        public async Task<IActionResult> GetSupplier([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {

            var request =
    await req.GetJsonBody<GetSupplierRequest, GetSupplierRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var supplierId = request.Value.SupplierId;

            var item = await _repository.GetAsync<Supplier>(supplierId, supplierId, _proxy.Suppliers)
                .ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Supplier[]), Description = "OK result")]
        [FunctionName(nameof(ListSuppliers))]
        public async Task<IActionResult> ListSuppliers([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {

            var request =
                await req.GetJsonBody<ListSuppliersRequest, ListSuppliersRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var items = await _repository.GetAsync<Supplier>(
                                  c => c.IsActive && (c.Type == nameof(Supplier)),
                                  _proxy.Suppliers)
                .ConfigureAwait(false);

            return new OkObjectResult(items);
        }

        [SwaggerResponse(200, typeof(Supplier), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(SupplierCreateRequest))]
        [FunctionName(nameof(SupplierCreate))]
        public async Task<IActionResult> SupplierCreate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<SupplierCreateRequest, SupplierCreateRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Supplier>();

            item = await _repository.CreateAsync(item, _proxy.Suppliers).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Supplier), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdateSupplierRequest))]
        [FunctionName(nameof(UpdateSupplier))]
        public async Task<IActionResult> UpdateSupplier([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<UpdateSupplierRequest, UpdateSupplierRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Supplier>();

            item = await _repository.UpdateAsync(item, _proxy.Suppliers).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}
