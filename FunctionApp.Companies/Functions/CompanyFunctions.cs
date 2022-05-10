using Core.Interfaces;
using Core.Models;
using FunctionApp.Companies.Requests;
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionApp.Companies.Functions
{
    public class CompanyFunctions
    {
        readonly ICosmosContainerProxy _proxy;

        readonly ICosmosRepository _repository;

        public CompanyFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _proxy = proxy;
            _repository = repository;
        }

        [SwaggerResponse(200, typeof(Company), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(CreateAddressRequest))]
        [FunctionName(nameof(CreateAddress))]
        public async Task<IActionResult> CreateAddress([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<CreateAddressRequest, CreateAddressRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var address = request.Value.Adapt<Address>();
            var item = await _repository.GetAsync<Company>(companyId, companyId, _proxy.Companies).ConfigureAwait(false);

            List<Address> addresses;
            if (item.Addresses == null)
            {
                addresses = new List<Address>();
                address.IsDefault = true;
            }
            else
            {
                addresses = item.Addresses;

                if (address.IsDefault)
                {
                    foreach (var existingAddress in addresses)
                    {
                        existingAddress.IsDefault = false;
                    }
                }
            }
            addresses.Add(address);

            item = await _repository.UpdateAsync(item, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Company), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(CreateCompanyRequest))]
        [FunctionName(nameof(CreateCompany))]
        public async Task<IActionResult> CreateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<CreateCompanyRequest, CreateCompanyRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = new Company { Id = Guid.NewGuid(), Name = request.Value.Name };

            item.PartnerId = new Guid(request.Value.PartnerId);

            item = await _repository.CreateAsync(item, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Company), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DeactivateCompanyRequest))]
        [FunctionName(nameof(DeactivateCompany))]
        public async Task<IActionResult> DeactivateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<DeactivateCompanyRequest, DeactivateCompanyRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;

            var item = await _repository.DeactivateAsync<Company>(companyId, companyId, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Company), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DeleteAddressRequest))]
        [FunctionName(nameof(DeleteAddress))]
        public async Task<IActionResult> DeleteAddress([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]

            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<DeleteAddressRequest, DeleteAddressRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var addressId = request.Value.AddressId;

            var item =
                    await _repository.GetAsync<Company>(companyId, companyId, _proxy.Companies).ConfigureAwait(false);

            if(!item.Addresses.Exists(a => a.Id == new Guid(addressId)))
            {
                return new NotFoundResult();
            }

            item.Addresses.RemoveAll(a => a.Id == new Guid(addressId));

            item = await _repository.UpdateAsync(item, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Company), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(GetCompanyRequest))]
        [FunctionName(nameof(GetCompany))]
        public async Task<IActionResult> GetCompany([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
    await req.GetJsonBody<GetCompanyRequest, GetCompanyRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;

            var item = await _repository.GetAsync<Company>(companyId, companyId, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Address[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ListAddressesRequest))]
        [FunctionName(nameof(ListAddresses))]
        public async Task<IActionResult> ListAddresses([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
await req.GetJsonBody<ListAddressesRequest, ListAddressesRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var isActive = request.Value.IsActive;

            var query = new QueryDefinition($"SELECT VALUE a FROM Companies c JOIN a in c.addresses WHERE c.id = @companyId AND a.isActive = @isActive")
                .WithParameter("@companyId", companyId)
                .WithParameter("@isActive", isActive);

            var items = await _repository.GetAsync<Address>(query, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(items);
        }

        [SwaggerResponse(200, typeof(Company[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ListCompaniesRequest))]
        [FunctionName(nameof(ListCompanies))]
        public async Task<IActionResult> ListCompanies([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest req, ILogger logger)
        {
            var request =
    await req.GetJsonBody<ListCompaniesRequest, ListCompaniesRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var partnerId = request.Value.PartnerId;
            var isActive = request.Value.IsActive;

            var companies = await _repository.GetAsync<Company>(c => c.PartnerId == new Guid(partnerId) && (c.IsActive == isActive), _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(companies);
        }

        [SwaggerResponse(200, typeof(Address), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdateAddressRequest))]
        [FunctionName(nameof(UpdateAddress))]
        public async Task<IActionResult> UpdateAddress([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<UpdateAddressRequest, UpdateAddressRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var address = request.Value.Adapt<Address>();
            var companyId = request.Value.CompanyId;

            var item = await _repository.GetAsync<Company>(companyId, companyId, _proxy.Companies).ConfigureAwait(false);
            item.Addresses.RemoveAll(a => a.Id == address.Id);
            item.Addresses.Add(address);

            item = await _repository.UpdateAsync(item, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Company), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(UpdateCompanyRequest))]
        [FunctionName(nameof(UpdateCompany))]
        public async Task<IActionResult> UpdateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request = await req.GetJsonBody<UpdateCompanyRequest, UpdateCompanyRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Company>();

            item = await _repository.UpdateAsync(item, _proxy.Companies).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}