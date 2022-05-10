using Core.Models;
using Core.Models.Tracking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Application.Models;
using Tradefact.Application.Tracking;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ShipmentController
    {
        protected readonly IMediator _mediator;
        public ShipmentController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(AISTrackingResult), Description = "OK Result")]
        [Route("{id:guid}/VesselTrack")]
        public async Task<IActionResult> GetVesselTrack(Guid id)
        {
            return new OkObjectResult(await _mediator.Send(new GetVesselAISPositionQuery { ShipmentId = id }));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<TrackingResource>), Description = "OK Result")]
        [Route("{id:guid}/Track")]
        public async Task<IActionResult> GetBillTRacking(Guid id)
        {
            return new OkObjectResult(await _mediator.Send(new GetBillTrackingQuery { ShipmentId = id }));
        }
    }
}
