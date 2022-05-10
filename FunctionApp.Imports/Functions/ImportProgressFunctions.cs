using System.Threading.Tasks;
using System.Web.Http;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using FunctionApp.Imports.Requests;
using Infrastructure.Functions.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Imports.Functions
{
    public class ImportProgressFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public ImportProgressFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(AddConsignmentDetailsRequest))]
        [FunctionName(nameof(AddConsignmentDetails))]
        public async Task<IActionResult> AddConsignmentDetails([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<AddConsignmentDetailsRequest, AddConsignmentDetailsRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            item.ConsignmentDetails = request.Value.Adapt<ImportConsignmentDetails>();

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(LoadedOnVesselRequest))]
        [FunctionName(nameof(LoadedOnVessel))]
        public async Task<IActionResult> LoadedOnVessel([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<LoadedOnVesselRequest, LoadedOnVesselRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            if(item.Status != ImportStatus.Collected) {
                return new BadRequestErrorMessageResult(
                           $"Cannot mark as Loaded on Vessel, an Import that has a status of {item.Status}");
            }

            item.Status = ImportStatus.Loaded;

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}
