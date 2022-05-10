using Core.Common;
using Core.Enums;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Models.Criteria;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Api.Infrastructure.Helpers;
using Tradefact.Api.Model;
using Tradefact.Api.Model.Shipment;
using Tradefact.Application.FreightMovements;
using Tradefact.Application.Models;
using Tradefact.Application.Models.Network;
using Tradefact.Application.Network.Queries;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest;
using Tradefact.Application.Shipments.Commands;
using Tradefact.Application.Shipments.Queries.GetExceptionsList;
using Tradefact.Application.Shipments.Queries.GetShipmentInfo;
using Tradefact.Application.Shipments.Queries.GetShipmentList;
using Tradefact.Application.Shipments.Queries.GetShipmentLocationsList;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    public partial class ShipperController : OrganisationController
    {
        private readonly ILogger<ShipperController> _logger;
        private readonly IMediator _mediator;
        private readonly SigningUrlHelper _signingUrlHelper;


        protected override OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.SHIPPER;
        public ShipperController(TradefactDbContext context, IMediator mediator, ITradefactActivityService tradefactActivityService, ILocationService locationService, ILogger<ShipperController> logger, SigningUrlHelper signingUrlHelper) : base(context, mediator, tradefactActivityService, locationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._signingUrlHelper = signingUrlHelper ?? throw new ArgumentNullException(nameof(signingUrlHelper));
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<QuotationRequestInfo>), Description = "Updated result")]
        [Route("QuotationRequest")]
        public async Task<IActionResult> GetQuotationRequests([FromQuery] PagedResultParameters @params, [FromQuery] QuotationSearchCriteria criteria)
        {
            IPagedList<QuotationRequestInfo> quotationRequests = await _mediator.Send(new GetQuotationRequestListQuery { 
                OrganisationId = this.OrganisationId, 
                OrganisationType = this.OrganisationType,
                Criteria = criteria,
                Paging = @params
            });
            return new OkObjectResult(new ListResource<QuotationRequestInfo>(quotationRequests));
        }

        protected string BuildSearchParam(string value)
        {
            return String.IsNullOrEmpty(value) ? null : $"%{value}%";
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(QuotationRequestResource), Description = "Updated result")]
        [Route("QuotationRequest/{id:guid}")]
        public async Task<IActionResult> GetQuotationRequestById(string id)
        {
            Guid quoteRequestId = new Guid(id);

            return Ok(await _mediator.Send(new GetQuotationRequestResourceQuery { QuotationRequestId = quoteRequestId, OrganisationId = this.OrganisationId, OrganisationType = this.OrganisationType }));
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(QuotationRequestResource), Description = "Updated result")]
        [Route("QuotationRequestItems/{id:guid}")]
        public async Task<IActionResult> QuotationRequestItems(string id)
        {
            Guid quoteRequestId = new Guid(id);

            return Ok(await _mediator.Send(new GetQuotationRequestItemsResourceQuery { QuotationRequestId = quoteRequestId, OrganisationId = this.OrganisationId, OrganisationType = this.OrganisationType }));
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(List<CargoItemResource>), Description = "Updated result")]
        [Route("QuotationRequestCargoItems")]
        public async Task<IActionResult> QuotationRequestCargoItemsById(string id, string freightMovementItemId, [FromQuery] PagedResultParameters @params, Guid? scheduleId = null)
        {
            Guid quoteRequestId = new Guid(id);

            IPagedList<CargoItemResource> cargoItemResources = await _mediator.Send(new GetQuotationRequestCargoItemsResourceQuery
            {
                QuotationRequestId = quoteRequestId,
                OrganisationId = this.OrganisationId,
                OrganisationType = this.OrganisationType,
                Paging = @params,
                ExistingFreightMovementItemId= freightMovementItemId,
                ScheduleId = scheduleId
            });

            return Ok(new ListResource<CargoItemResource>(cargoItemResources));
        }



        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Route("Shipment")]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
        {
            var createShipment = new CreateShipmentCommand(this.OrganisationId, this.OrganisationType, request.QuotationRequestId, request.Route);
            _logger.LogInformation(
                "----- Sending command: Create Shipment - {IdProperty}", createShipment.QuotationRequestId);
            Guid shipmentId = await _mediator.Send(createShipment);

            return new OkObjectResult(new { id = shipmentId });
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<QuotationRequestInfo>), Description = "Updated result")]
        [Route("Shipments")]
        public async Task<IActionResult> GetShipments([FromQuery] PagedResultParameters @params, [FromQuery] ShipmentSearchCriteria criteria)
        {
            if (criteria.ShowExceptionDetailsOnly)
            {
                IPagedList<ExceptionsInfo> exceptions = await _mediator.Send(new GetExceptionsListQuery
                {
                    OrganisationId = this.OrganisationId,
                    OrganisationType = this.OrganisationType,
                    Criteria = criteria,
                    Paging = @params
                });
                return new OkObjectResult(new ListResource<ExceptionsInfo>(exceptions));
            }

            if (criteria.HasGeoCoordinates)
            {
                IPagedList<ShipmentMapInfo> shipmentsWithMap = await _mediator.Send(new GetShipmentLocationsListQuery
                {
                    OrganisationId = this.OrganisationId,
                    OrganisationType = this.OrganisationType,
                    Criteria = criteria,
                });

                return new OkObjectResult(new ListResource<ShipmentMapInfo>(shipmentsWithMap));
            }

            IPagedList<ShipmentInfo> shipments = await _mediator.Send(new GetShipmentListQuery
            {
                OrganisationId = this.OrganisationId,
                OrganisationType = this.OrganisationType,
                Criteria = criteria,
                Paging = @params
            });

            return new OkObjectResult(new ListResource<ShipmentInfo>(shipments));
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/{id:guid}")]
        public async Task<IActionResult> GetShipmentById(Guid id)
        {
            ShipmentInfo s = await GetShipmentinfo(id, this.OrganisationId);

            QuotationRequestResource qr = await _mediator.Send(new GetQuotationRequestResourceQuery { QuotationRequestId = s.QuotationRequestId, OrganisationId = this.OrganisationId, OrganisationType = this.OrganisationType });
            s.FreightMovement = qr.FreightMovement;
            s.Quotation = qr.Quotation;

            return new OkObjectResult(s);
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(string[]), Description = "Updated result")]
        [DisableFormValueModelBinding]
        [Route("shipment/updatetags")]
        public async Task<IActionResult> Shipment([FromBody] AssignShipmentTagsRequest request)
        {
            Shipment shipment = await _context.Shipments.SingleOrDefaultAsync(q => q.Id == request.ShipmentId);
            if (shipment == null)
            {
                return new NotFoundResult();
            }

            shipment.Tags = String.Join(',', request.Tags);
            _ = await _context.SaveChangesAsync();


            ShipmentInfo s = await GetShipmentinfo(shipment.Id,this.OrganisationId);

            return new OkObjectResult(s.TagsList);
        }

        private async Task<ShipmentInfo> GetShipmentinfo(Guid shipmentId, Guid clientId) {
            ShipmentInfo s = await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = shipmentId, ClientId = clientId });
            s.PackingListURL = _signingUrlHelper.GenerateSignedURL("PackingList", $"sid={s.Id.ToString()}");
            s.PackingListReady = false;
            return s;
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/{id:guid}/csv")]
        public async Task<FileStreamResult> GetShipmentCSVById(Guid id)
        {
            ShipmentInfo s;
            Organisation org = _context.Organisations.First(x => x.Id == this.OrganisationId);
            if (org.OrganisationTypeId == OrganisationTypeEnum.SHIPPER) 
                s = await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ClientId = this.OrganisationId });
            else 
                s = await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId });
            QuotationRequestResource qr = await _mediator.Send(new GetQuotationRequestResourceQuery { QuotationRequestId = s.QuotationRequestId, OrganisationId = this.OrganisationId, OrganisationType = this.OrganisationType });


            var data = qr.FreightMovement.Items.SelectMany(s => s.CargoItems.Select(c => new { Product = c.Product.Name, SKU = c.Product.SKU, Quantity = c.Qty }));


            byte[] byteArray = Encoding.ASCII.GetBytes(data.GenerateCSV());
            MemoryStream stream = new MemoryStream(byteArray);

            return new FileStreamResult(stream, "application/vnd.ms-excel")
            {
                FileDownloadName = "PurchaseOrderExport.csv"
            };
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(QuotationRequestResource), Description = "Updated result")]
        [Route("QuotationRequest/{id:guid}/csv")]
        public async Task<FileStreamResult> GetQuotationRequestCSVById(string id)
        {
            Guid quoteRequestId = new Guid(id);
            Organisation org = _context.Organisations.First(x => x.Id == this.OrganisationId);
            QuotationRequestResource q = await _mediator.Send(new GetQuotationRequestResourceQuery { QuotationRequestId = quoteRequestId, OrganisationId = this.OrganisationId, OrganisationType = org.OrganisationTypeId });

            var data = q.FreightMovement.Items.SelectMany(s => s.CargoItems.Select(c => new { Product = c.Product.Name, SKU = c.Product.SKU, Quantity = c.Qty }));
                //.Union(q.FreightMovement.LCL.Select(s => new { Product = s.Product.Name, SKU = s.Product.SKU, Quantity = s.Qty })); 

            byte[] byteArray = Encoding.ASCII.GetBytes(data.GenerateCSV());
            MemoryStream stream = new MemoryStream(byteArray);

            return new FileStreamResult(stream, "application/vnd.ms-excel")
            {
                FileDownloadName = "PurchaseOrderExport.csv"
            };
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<LogisticsClient>), Description = "OK result")]
        [Route("LogisticsClients")]
        public async Task<IActionResult> GetLogisticsClients([FromQuery] PagedResultParameters @params, [FromQuery] OrganisationSearchCriteria criteria)
        {
            IPagedList<LogisticsClient> clients = await _mediator.Send(new GetOrganisationsPerForwarderListQuery
            {
                OrganisationId = this.OrganisationId,
                Criteria = criteria,
                CompanyType = "ProviderId",
                RequestingType = "ClientId",
                InviteType = (int)InviteType.NewFreightForwarderInvitedByShipper,
                Paging = @params
            });
            
            if (clients == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(new ListResource<LogisticsClient>(clients));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(NetworkResource), Description = "OK result")]
        [Route("Network")]
        public async Task<IActionResult> GetNetwork()
        {
            NetworkResource network = await _mediator.Send(new GetShipperNetworkListQuery
            {
                OrganisationId = this.OrganisationId,
            });

            if (network == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(network);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<CargoItemResource>), Description = "Ok")]
        [Route("ShipmentCargo")]
        public async Task<IActionResult> GetShipmentCargo([FromQuery] PagedResultParameters @params, Guid id, string search = null)
        {
            IPagedList<CargoItemResource> result = await _mediator.Send(new GetFreightMovementCargoItemsByIdQuery
            {
                FreightMovementId = id,
                OrganisationId = this.OrganisationId,
                Paging=@params,
                Search=search
            });
            return this.HandleSuccessResponse(new ListResource<CargoItemResource>(result));
        }

    }
}