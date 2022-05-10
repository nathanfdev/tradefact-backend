using Core.Common;
using Flare.Api.Model;
using Flare.Data;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;


namespace Flare.Api.Controllers
{
    public class OrderController : BaseApiController
    {
        public OrderController(FlareDbContext context) : base(context)
        {
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<OrderResource>), Description = "OK result")]
        public virtual async Task<IActionResult> Get([FromQuery] PagedResultParameters @params, bool active = true)
        {
            OrderStatus[] wantStatus = active ?
                new[] { OrderStatus.AwaitingFulfillment, OrderStatus.TrackingActive } :
                new[] { OrderStatus.TrackingComplete };

            IPagedList<OrderResource> orders = await _context.Orders
                .Include(o => o.LineItems)
                    .ThenInclude(i => i.Device)
                        .ThenInclude(d => d.LastDeviceReport)
                .Where(q => q.OrganisationId == this.OrganisationId && wantStatus.Contains(q.OrderStatus))
                .OrderByDescending(o => o.ShipmentDate)
                .Adapt<List<OrderResource>>()
                .ToPagedListAsync(@params.PageNumber, @params.PageSize);

            return new OkObjectResult(new ListResource<OrderResource>(orders));
        }

        [HttpGet("{id}")]
        [SwaggerResponse("200", typeof(OrderResource), Description = "OK result")]
        public virtual async Task<IActionResult> GetById(string id)
        {
            Order order = await _context.Orders
                .Include(o => o.LineItems)
                    .ThenInclude(i => i.Device)
                        .ThenInclude(d => d.LastDeviceReport)
                .FirstOrDefaultAsync(q => q.OrganisationId == this.OrganisationId && q.Id == new Guid(id));

            if (order == null)
            {
                return new NotFoundResult();
            }
            
            return new OkObjectResult(order.Adapt<OrderResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(OrderResource), Description = "Created result")]
        public virtual async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            Order order = request.Adapt<Order>();
            order.Id = Guid.NewGuid();
            order.OrganisationId = this.OrganisationId;

            _context.Orders.Add(order);
            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(order);
        }
    }
}
