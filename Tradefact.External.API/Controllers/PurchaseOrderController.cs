using Core.Common;
using Core.Models.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mapster;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tradefact.Data;
using Tradefact.External.API.Model;
using X.PagedList;

namespace Tradefact.External.API.Controllers
{
    public class PurchaseOrderController : BaseAPIController
    {
        private PagedResultParameters GetPagingRequirements(PagingParams @params) => @params == null ? new PagedResultParameters() { NoPage = false, PageNumber = 1, PageSize = 100 } : new PagedResultParameters() { NoPage = false, PageNumber = @params.PageNumber, PageSize = @params.PageSize };

        private readonly ILogger<PurchaseOrderController> _logger;
        public PurchaseOrderController(TradefactDbContext context, ILogger<PurchaseOrderController> logger) : base(context)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(ListResource<PurchaseOrder>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromQuery] PagingParams @params = null)
        {
            PagedResultParameters paging = this.GetPagingRequirements(@params);
            List<PurchaseOrder> orders = new List<PurchaseOrder>();
            return new OkObjectResult(new ListResource<PurchaseOrder>(new StaticPagedList<PurchaseOrder>(orders, paging.PageNumber, paging.PageSize, orders.Count)));
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(PurchaseOrder), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PurchaseOrder>> ItemByIdAsync(string id)
        {
            if (id is null || String.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var item = new PurchaseOrder { PurchaseOrderID = Guid.NewGuid().ToString() };

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] PurchaseOrder purchaseOrderToCreate)
        {
            if (!this.CheckApiKey()) return Unauthorized();

            ExternalPurchaseOrder externalOrder = purchaseOrderToCreate.Adapt<ExternalPurchaseOrder>();
            externalOrder.Id = Guid.NewGuid();
            externalOrder.Received = this.ActionTime;
            externalOrder.OrganisationId = this.OrganisationId;
            externalOrder.Source = "API";

            foreach (var line_item in purchaseOrderToCreate.LineItems.Select(s => s.Adapt<ExternalPurchaseOrderLineItem>()))
            {
                line_item.ExternalPurchaseOrderId = externalOrder.Id;
                line_item.Id = Guid.NewGuid();
                externalOrder.LineItems.Add(line_item);
            };

            _context.ExternalPurchaseOrders.Add(externalOrder);
            _ = await _context.SaveChangesAsync();

            return Created(nameof(PurchaseOrderController), new { PurchaseOrderID = externalOrder.Id });
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateProductAsync([FromBody] PurchaseOrder purchaseOrderToUpdate)
        {
            //var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            //if (catalogItem == null)
            //{
            //    return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            //}

            //var oldPrice = catalogItem.Price;
            //var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            //// Update current product
            //catalogItem = productToUpdate;
            //_catalogContext.CatalogItems.Update(catalogItem);

            //if (raiseProductPriceChangedEvent) // Save product's data and publish integration event through the Event Bus if price has changed
            //{
            //    //Create Integration Event to be published through the Event Bus
            //    var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice);

            //    // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            //    await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

            //    // Publish through the Event Bus and mark the saved event as published
            //    await _catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            //}
            //else // Just save the updated product because the Product's Price hasn't changed.
            //{
            //    await _catalogContext.SaveChangesAsync();
            //}
            return Created(nameof(PurchaseOrderController), new { id = purchaseOrderToUpdate.PurchaseOrderID });
        }

    }
}
