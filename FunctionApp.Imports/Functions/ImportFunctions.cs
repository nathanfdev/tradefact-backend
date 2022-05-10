using Bogus;
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
using System.Threading.Tasks;

namespace FunctionApp.Imports.Functions
{
    public class ImportFunctions
    {
        readonly ICosmosContainerProxy _proxy;

        readonly ICosmosRepository _repository;

        public ImportFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(AssignQuotationRequest))]
        [FunctionName(nameof(AssignQuotation))]
        public async Task<IActionResult> AssignQuotation([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                    await req.GetJsonBody<AssignQuotationRequest, AssignQuotationRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);


            item.Quotations[0] = request.Value.Adapt<ImportQuotation>();

            item.Company = await _repository.GetAsync<Company>(companyId, companyId, _proxy.Companies).ConfigureAwait(false);

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(AssignSupplierRequest))]
        [FunctionName(nameof(AssignSupplier))]
        public async Task<IActionResult> AssignSupplier([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
        await req.GetJsonBody<AssignSupplierRequest, AssignSupplierRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;
            var supplierId = request.Value.SupplierId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);
            // item.Supplier = await _repository.GetAsync<Supplier>(supplierId, supplierId, _proxy.Suppliers).ConfigureAwait(false);

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(CreateImportRequest))]
        [FunctionName(nameof(CreateImport))]
        public async Task<IActionResult> CreateImport([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<CreateImportRequest, CreateImportRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Import>();

            var company = await _repository.GetAsync<Company>(item.CompanyId.ToString(), item.CompanyId.ToString(), _proxy.Companies).ConfigureAwait(false);

            item.Company = company;
            item.Status = ImportStatus.Created;

            item = await _repository.CreateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, null, Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DeactivateImportRequest))]
        [FunctionName(nameof(DeactivateImport))]
        public async Task<IActionResult> DeactivateImport([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<DeactivateImportRequest, DeactivateImportRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            await _repository.DeactivateAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            return new OkResult();
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(GetImportRequest))]
        [FunctionName(nameof(GetImport))]
        public async Task<IActionResult> GetImport([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<GetImportRequest, GetImportRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Import[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ListImportsRequest))]
        [FunctionName(nameof(ListImports))]
        public async Task<IActionResult> ListImports([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<ListImportsRequest, ListImportsRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var isActive = request.Value.IsActive;
            var items = await _repository.GetAsync<Import>(c => (c.CompanyId == new System.Guid(companyId)) && (c.IsActive == isActive) && (c.Type == nameof(Import)), _proxy.Imports)
                .ConfigureAwait(false);


            foreach(var item in items)
            {
                var faker = new Faker<Import>().RuleFor(f => f.ETA, f => f.Date.Soon()).Generate();

                item.ETA = faker.ETA;
            }

            return new OkObjectResult(items);
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdateImportRequest))]
        [FunctionName(nameof(UpdateImport))]
        public async Task<IActionResult> UpdateImport([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<UpdateImportRequest, UpdateImportRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Import>();

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}