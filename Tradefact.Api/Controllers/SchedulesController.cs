using Core.Common;
using Core.Models.Criteria;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Application.Schedules.Commands;
using Tradefact.Application.Schedules.Model;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : BaseApiController
    {

        private readonly ILogger<PurchaseOrderController> _logger;
        protected readonly IMediator _mediator;

        public SchedulesController(TradefactDbContext context, IMediator mediator, ILogger<SchedulesController> logger) : base(context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        private async Task<PurchaseOrderScheduleLineResource> ByIdAsync(Guid id, bool includeItems = default, [FromQuery] PagedResultParameters @params = default)
        {
            PurchaseOrderScheduleLineResource schedule = await _mediator.Send(new GetPurchaseOrderScheduleByIdQuery
            {
                ScheduleLineId = id,
                IncludeItems = includeItems,
                Paging = @params
            });
            return schedule;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<PurchaseOrderScheduleLineResource>), Description = "OK result")]
        public async Task<IActionResult> Get([FromQuery] PagedResultParameters @params, [FromQuery] SchedulesSearchCriteria criteria)
        {
            IPagedList<PurchaseOrderScheduleLineResource> schedulelines = await _mediator.Send(new GetPurchaseOrderSchedulesQuery
            {
                OrganisationId = this.OrganisationId,
                Criteria = criteria,
                Paging = @params
            });
            return new OkObjectResult(new ListResource<PurchaseOrderScheduleLineResource>(schedulelines));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(PurchaseOrderScheduleLineResource), Description = "OK result")]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromQuery] PagedResultParameters @params, Guid id, bool includeItems = default)
        {
            PurchaseOrderScheduleLineResource resource = await this.ByIdAsync(id, includeItems, @params);
            return new OkObjectResult(resource);
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderScheduleLineResource), Description = "Created result")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderScheduleCommand createPurchaseOrderScheduleCommand)
        {
            createPurchaseOrderScheduleCommand.OrganisationId = this.OrganisationId;
            Guid id = await _mediator.Send(createPurchaseOrderScheduleCommand);

            return new OkObjectResult(await this.ByIdAsync(id, false));
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(PurchaseOrderScheduleLineResource), Description = "Update result")]
        [Route("{id:guid}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var itemId = await _mediator.Send(new DeletePurchaseOrderScheduleCommand(this.OrganisationId, id));
            return new OkObjectResult(new { });
        }

    }
}
