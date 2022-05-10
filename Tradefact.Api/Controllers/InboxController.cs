using Core.Common;
using Core.Models;
using Core.Models.External;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InboxController : BaseApiController
    {
        private readonly ILogger<StatusController> _logger;
        protected readonly IMediator _mediator;
        private readonly ICurrentUserService _userService;

        public InboxController(TradefactDbContext context, IMediator mediator, ILogger<StatusController> logger, ICurrentUserService userService) : base(context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this._logger = logger;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<InboxPurchaseOrderResource>), Description = "OK result")]
        [Route("PurchaseOrders")]
        public async Task<IActionResult> GetPurchaseOrderInbox([FromQuery] InboxPurchaseOrderSearchCriteria criteria, [FromQuery] PagedResultParameters @params)
        {
            IPagedList<InboxPurchaseOrderResource> result = await _mediator.Send(new GetAllInboxPurchaseOrdersQuery
            {
                OrganisationId = this.OrganisationId,
                Criteria = criteria,
                Paging = @params
            });

            return new OkObjectResult(new ListResource<InboxPurchaseOrderResource>(result));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<InboxPurchaseOrderLineItemResource>), Description = "OK Result")]
        [Route("PurchaseOrders/{id:guid}/Items")]
        public async Task<IActionResult> GetItems(Guid id, [FromQuery] PagedResultParameters @params)
        {
            List<InboxPurchaseOrderLineItemResource> items = await _mediator.Send(new GetInboxPurchaseOrderLineItemsQuery
            {
                OrganisationId = this.OrganisationId,
                ExternalPurchaseOrderId = id
            });

            var pagedItems = await items.ToPagedListAsync(@params.PageNumber, @params.PageSize);

            return new OkObjectResult(new ListResource<InboxPurchaseOrderLineItemResource>(pagedItems));
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(InboxPurchaseOrderResource), Description = "OK Result")]
        [Route("PurchaseOrders/{id:guid}")]
        public async Task<IActionResult> UpdatePurchaseOrderInbox(Guid id, [FromBody]InboxPurchaseOrderResource request)
        {
            ExternalPurchaseOrder xpo = await _context.ExternalPurchaseOrders.Include(q => q.LineItems).SingleOrDefaultAsync(q => q.Id == id);
            if (xpo == null) return new NotFoundResult();

            xpo.PurchaseOrderNumber = request.PurchaseOrderNumber;
            xpo.GenericProductId = request.GenericProductId;
            xpo.SupplierId = request.SupplierId;


            Product product = await _context.Products.FindAsync(request.GenericProductId);

            xpo.LineItems.ForEach(li =>
            {
                var doesSKUExistInProducts = _context.Products.Any(q => q.SKU == li.SKU && q.CompanyId == this.OrganisationId && q.IsActive);

                if (!doesSKUExistInProducts)
                {
                    li.SKU = product.SKU;
                }
            });

            _ = await _context.SaveChangesAsync();

            InboxPurchaseOrderResource result = (await _mediator.Send(new GetAllInboxPurchaseOrdersQuery
            {
                OrganisationId = this.OrganisationId,
                Paging = new PagedResultParameters { NoPage = true }
            })).FirstOrDefault(q => q.Id == id);

            return new OkObjectResult(result);
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(InboxPurchaseOrderResource), Description = "Deleted Result")]
        [Route("PurchaseOrders/{id:guid}")]
        public async Task<IActionResult> DeletePurchaseOrderInbox(Guid id)
        {
            ExternalPurchaseOrder xpo = await _context.ExternalPurchaseOrders.FindAsync(id);
            if (xpo == null) return new NotFoundResult();

            xpo.IsActive = false;

            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(new { });
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(InboxPurchaseOrderResource), Description = "OK Result")]
        [Route("PurchaseOrders/Import")]
        public async Task<IActionResult> ImportExternalPurchaseOrders([FromBody] List<InboxPurchaseOrderResource> SelectedOrders)
        {
            if (SelectedOrders != null)
            {
                foreach (InboxPurchaseOrderResource SelectedOrder in SelectedOrders)
                {
                    ExternalPurchaseOrder xpo = await _context.ExternalPurchaseOrders
                        .Include(i => i.LineItems)
                        .FirstOrDefaultAsync(q => q.Id == SelectedOrder.Id && q.OrganisationId == this.OrganisationId);
                    if (xpo == null) return new NotFoundResult();

                    xpo.SupplierId = SelectedOrder.SupplierId;
                    xpo.SupplierName = SelectedOrder.SupplierName;

                    PurchaseOrder po = xpo.Adapt<PurchaseOrder>();
                    foreach (PurchaseOrderItem item in po.PurchaseOrderItems)
                    {
                        item.PurchaseOrderId = xpo.Id;
                        Product product = _context.Products.FirstOrDefault(q => q.CompanyId == this.OrganisationId && q.SKU == item.SKU && q.IsActive);

                        if (xpo.GenericProductId.HasValue)
                        {
                            item.ProductId = Guid.Empty;
                        }
                        else if (product != null)
                        {
                            item.ProductId = product.Id;
                            item.SKU = null;
                            item.PurchaseOrderItemText = null;
                        }
                    }

                    _context.PurchaseOrders.Add(po);

                    xpo.Imported = true;
                    xpo.ImportDate = DateTime.Now;
                    xpo.ImportUserId = new Guid(_userService.UserId);
                    xpo.ImportUserName = _userService.Name;

                    // Each purchase order is automatically assigned a comments room
                    Room r = new Room
                    {
                        Id = po.Id,
                        IsOpen = true,
                        UnreadCount = 0,
                    };
                    _context.Rooms.Add(r);
                    _ = await _context.SaveChangesAsync();

                    _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(po.Id, PurchaseOrderEventType.Created, DateTime.Now));

                    _ = await _mediator.Send(new PurchaseOrderItemProductSupplierCreateCommand
                    {
                        PurchaseOrderId = po.Id,
                        LastChangeUser = po.LastChangeUser
                    });
                }
            }

            return new OkObjectResult("OK");
        }
    }
}
