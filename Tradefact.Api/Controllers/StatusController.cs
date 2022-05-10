using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : BaseApiController
    {
        private readonly ILogger<StatusController> _logger;
        protected readonly IMediator _mediator;
        private readonly AppSettings AppSettings;

        public StatusController(TradefactDbContext context, IMediator mediator, ILogger<StatusController> logger) : base(context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._logger = logger;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(OrganisationStatusResource), Description = "OK Result")]
        public async Task<IActionResult> Get()
        {
            OrganisationStatusResource status = await _mediator.Send(new GetOrganisationalStatusQuery
            {
                OrganisationId = this.OrganisationId,
            });
            return new OkObjectResult(status);
        }

    }
}
