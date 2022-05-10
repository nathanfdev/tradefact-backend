using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using FunctionApp.Partners.Requests;
using Infrastructure.Functions.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Partners.Functions
{
    public class PartnerFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public PartnerFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(Partner), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(CreateOrUpdateAddressRequest))]
        [FunctionName(nameof(CreateOrUpdateAddress))]
        public async Task<IActionResult> CreateOrUpdateAddress([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<CreateOrUpdateAddressRequest, CreateOrUpdateAddressRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            string partnerId = request.Value.PartnerId;
            var address = request.Value.Adapt<Address>();

            address.Id = Guid.NewGuid();

            var item = await _repository.GetAsync<Partner>(partnerId, partnerId, _proxy.Partner).ConfigureAwait(false);

            item.Address = address;

            item = await _repository.UpdateAsync(item, _proxy.Partner).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Partner), Description = "Created result")]
        [SwaggerRequestBodyType(typeof(CreatePartnerRequest))]
        [FunctionName(nameof(CreatePartner))]
        public async Task<IActionResult> CreatePartner([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<CreatePartnerRequest, CreatePartnerRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Partner>();

            item = await _repository.CreateAsync(item, _proxy.Partner).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, null, Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DeactivatePartnerRequest))]
        [FunctionName(nameof(DeactivatePartner))]
        public async Task<IActionResult> DeactivatePartner([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<DeactivatePartnerRequest, DeactivatePartnerRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            string partnerId = request.Value.PartnerId;

            var item = await _repository.DeactivateAsync<Partner>(partnerId, partnerId, _proxy.Partner)
                .ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Partner), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(GetPartnerRequest))]
        [FunctionName(nameof(GetPartner))]
        public async Task<IActionResult> GetPartner([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<GetPartnerRequest, GetPartnerRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var partnerId = request.Value.PartnerId;

            var item = await _repository.GetAsync<Partner>(partnerId, partnerId, _proxy.Partner).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Partner[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ListPartnersRequest))]
        [FunctionName(nameof(ListPartners))]
        public async Task<IActionResult> ListPartners([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {

            var request =
    await req.GetJsonBody<ListPartnersRequest, ListPartnersRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var items = await _repository.GetAsync<Partner>(
                                  c => (c.IsActive == request.Value.IsActive) && (c.Type == nameof(Partner)),
                                  _proxy.Partner)
                .ConfigureAwait(false);

            return new OkObjectResult(items);
        }

        [SwaggerResponse(200, typeof(Partner), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ReactivatePartnerRequest))]
        [FunctionName(nameof(ReactivatePartner))]
        public async Task<IActionResult> ReactivatePartner([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<ReactivatePartnerRequest, ReactivatePartnerRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            string partnerId = request.Value.PartnerId;

            var item = await _repository.ReactivateAsync<Partner>(partnerId, partnerId, _proxy.Partner)
                .ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Partner), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdatePartnerRequest))]
        [FunctionName(nameof(UpdatePartner))]
        public async Task<IActionResult> UpdatePartner([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<UpdatePartnerRequest, UpdatePartnerRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Partner>();

            item = await _repository.UpdateAsync(item, _proxy.Partner).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}
