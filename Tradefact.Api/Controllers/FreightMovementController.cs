using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Core.ServiceBus;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Tradefact.Api.Infrastructure.Extensions;
using Tradefact.Api.Responses;
using Tradefact.Api.Services;
using Tradefact.Application.FreightMovements;
using Tradefact.Application.Models;
using Tradefact.Data;
using Tradfact.Api.Requests;


namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreightMovementController : BaseApiController
    {
        private readonly IServiceBusClient _serviceBusClient;
        private readonly ITradefactActivityService _tradefactActivityService;
        protected readonly IMediator _mediator;

        public FreightMovementController(TradefactDbContext context, IMediator mediator, IServiceBusClient serviceBusClient, ITradefactActivityService tradefactActivityService) : base(context) 
        {
            _serviceBusClient = serviceBusClient;
            _tradefactActivityService = tradefactActivityService;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        private async Task<FreightMovementResource> ByIdAsync(Guid freightMovementId)
        {
            return await _mediator.Send(new GetFreightMovementByIdQuery
            {
                FreightMovementId = freightMovementId,
                OrganisationId = this.OrganisationId
            });
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(List<FreightMovementResource>), Description = "OK Result")]
        [Route(nameof(Get))]
        public async Task<IActionResult> Get()
        {
            Guid organisationId = this.User.GetUserOrganisationId();

            var movements = await _context.FreightMovements
                .Where(q => q.CompanyId == organisationId && q.IsActive).ToListAsync();
            return new OkObjectResult(movements.OrderBy(o => o.CreationDate).Adapt<FreightMovementResource[]>());
        }

        [HttpGet("{id}")]
        [SwaggerResponse("200", typeof(FreightMovementResource), Description = "OK Result")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return new OkObjectResult(await this.ByIdAsync(id));
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(FreightMovementResource), Description = "Created result")]
        [Route(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] CreateFreightMovementCommand request) //CreateFreightMovementCommand CreateFreightMovementRequest
        {
            request.OrganisationId = this.OrganisationId;
            Guid id = await _mediator.Send(request);

            await _tradefactActivityService.TrackEvent(User, this.OrganisationId, "quote_requested", new TrackWith { Segment = true, Tradefact = false }, new EventProps { Segment = request });

            return new OkObjectResult(await this.ByIdAsync(id));
        }

        private async Task<Guid> GetProductVariantMasterId(Guid variantId)
        {
            ProductVariant variant = await _context.ProductVariants.SingleOrDefaultAsync(s => s.Id == variantId);
            return variant.ProductId; 
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(FreightMovementResource), Description = "Created result")]
        [Route(nameof(Update))]
        public async Task<IActionResult> Update([FromBody]UpdateFreightMovementRequest request)
        {
            FreightMovement item = await _context.FreightMovements.FindAsync(request.RequestId);
            item = request.Adapt<FreightMovement>();
            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(item.Adapt<FreightMovementResource>());
        }

        private async void SendMessageToServiceBus(FreightMovement fm, Organisation org, Organisation shipper)
        {
            string QueueName = "quotequeue";

            ServiceBusMessage<QuoteCreatedMail> msg = new ServiceBusMessage<QuoteCreatedMail>(new QuoteCreatedMail
            {
                Email = org.ContactEmail,
                OrganisationName = shipper.Name,
                FirstName = org.ContactName
            });

            await _serviceBusClient.Publish<QuoteCreatedMail>(msg, QueueName);
        }
    }
}