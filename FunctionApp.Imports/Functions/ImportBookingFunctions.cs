using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using FunctionApp.Imports.Requests;
using Infrastructure.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Imports.Functions
{
    public class ImportBookingFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public ImportBookingFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(AcceptBookingRequest))]
        [FunctionName(nameof(AcceptBooking))]
        public async Task<IActionResult> AcceptBooking([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<AcceptBookingRequest, AcceptBookingRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            if(item.Status != ImportStatus.PreBooking) {
                return new BadRequestErrorMessageResult(
                           $"Cannot Accept Booking of an Import that has a status of {item.Status}");
            }

            item.Status = ImportStatus.Booked;

            ImportQuotation q = item.Quotations.First();

            q.DateIssued = DateTime.Today;
            q.DateDue = DateTime.Today.AddDays(30);

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(BookImportRequest))]
        [FunctionName(nameof(BookImport))]
        public async Task<IActionResult> BookImport([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<BookImportRequest, BookImportRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            if(item.Status != ImportStatus.Created) {
                return new BadRequestErrorMessageResult($"Cannot Book an Import that has a status of {item.Status}");
            }

            item.PurchaseOrders = request.Value.PurchaseOrders;
            item.Status = ImportStatus.PreBooking;

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, typeof(Import), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(RejectBookingRequest))]
        [FunctionName(nameof(RejectBooking))]
        public async Task<IActionResult> RejectBooking([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<RejectBookingRequest, RejectBookingRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;

            var item = await _repository.GetAsync<Import>(importId, companyId, _proxy.Imports).ConfigureAwait(false);

            if(item.Status != ImportStatus.PreBooking) {
                return new BadRequestErrorMessageResult(
                           $"Cannot Reject Booking of an Import that has a status of {item.Status}");
            }

            item.Status = ImportStatus.Rejected;

            item = await _repository.UpdateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }
    }
}
