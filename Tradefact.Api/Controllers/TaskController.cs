using Core.Dtos;
using Core.Dtos.Product;
using Core.Dtos.PurchaseOrder;
using Core.Interfaces;
using Core.Models;
using Core.ServiceBus;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    public class TaskController : BaseApiController
    {
        private readonly IServiceBusClient _serviceBusClient;
        private readonly ITradefactActivityService _tradefactActivityService;

        public TaskController(TradefactDbContext context, IServiceBusClient serviceBusClient, ITradefactActivityService tradefactActivityService) : base(context)
        {
            _serviceBusClient = serviceBusClient;
            _tradefactActivityService = tradefactActivityService;
        }

        [HttpGet("{id}")]
        [SwaggerResponse("200", typeof(QueuedTaskResource), Description = "OK Result")]
        public async Task<IActionResult> GetById(string id)
        {
            QueuedTask t = await _context.QueuedTasks.SingleAsync(q => q.Id == new Guid(id));
            if (t == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(t.Adapt<QueuedTaskResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(QueuedTaskResource), Description = "Created result")]
        [Route("flatfile/purchaseorder/import")]
        public async Task<IActionResult> Post([FromBody] PurchaseOrderFlatfileImportRequest request)
        {
            string import_type = request.PurchaseOrderId.HasValue ? "Direct" : "External";

            QueuedTask t = new QueuedTask
            {
                Id = Guid.NewGuid(),
                Status = Core.Enums.QueuedTaskStatus.Pending,
                Description = $"Flatfile Import, { import_type } Purchase Order: {request.PurchaseOrderId.GetValueOrDefault()} - BatchId {request.BatchId}",
            };
            _context.QueuedTasks.Add(t);
            _ = await _context.SaveChangesAsync();

            PurchaseOrderFlatfileImport payload = new PurchaseOrderFlatfileImport
            {
                CorrelationId = t.Id,
                PurchaseOrderId = request.PurchaseOrderId,
                BatchId = request.BatchId,
                UserId = t.CreatedByUser,
                ProductId = request.ProductId,
                SupplierId = request.SupplierId,
                PurchaseOrderNo = request.PurchaseOrderNo,
                CurrencyCode = request.CurrencyCode,
                OrganisationId = this.OrganisationId
            };
            _ = await _context.SaveChangesAsync();

            t.Payload = JsonConvert.SerializeObject(payload);

            string QueueName = request.PurchaseOrderId.HasValue ? "flatfilepoimport" : "externalflatfilepoimport";

            ServiceBusMessage<PurchaseOrderFlatfileImport> msg = new ServiceBusMessage<PurchaseOrderFlatfileImport>(payload);
            await _serviceBusClient.Publish<PurchaseOrderFlatfileImport>(msg, QueueName);

            return new OkObjectResult(t.Adapt<QueuedTaskResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(QueuedTaskResource), Description = "Created result")]
        [Route("flatfile/product/import")]
        public async Task<IActionResult> ProductCSVImport([FromBody] ProductFlatfileImportRequest request)
        {
            QueuedTask t = new QueuedTask
            {
                Id = Guid.NewGuid(),
                Status = Core.Enums.QueuedTaskStatus.Pending,
                Description = $"Flatfile Import, Products: BatchId {request.BatchId}",
            };
            _context.QueuedTasks.Add(t);
            _ = await _context.SaveChangesAsync();

            ProductFlatfileImport payload = new ProductFlatfileImport
            {
                CorrelationId = t.Id,
                BatchId = request.BatchId,
                UserId = t.CreatedByUser,
                ProductId = request.ProductId.HasValue ? request.ProductId.GetValueOrDefault() : null,
                CompanyId = this.OrganisationId,
                SupplierId = request.SupplierId.HasValue ? request.SupplierId.GetValueOrDefault() : null
            };
            _ = await _context.SaveChangesAsync();

            t.Payload = JsonConvert.SerializeObject(payload);

            string QueueName = "flatfileproductimport";

            ServiceBusMessage<ProductFlatfileImport> msg = new ServiceBusMessage<ProductFlatfileImport>(payload);
            await _serviceBusClient.Publish<ProductFlatfileImport>(msg, QueueName);

            await _tradefactActivityService.TrackEvent(User, this.OrganisationId, "bulk_import_to_catalogue", new TrackWith { Segment = true, Tradefact = false }, new EventProps { Segment = request });

            return new OkObjectResult(t.Adapt<QueuedTaskResource>());
        }


    }
}
