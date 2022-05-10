using Core.Common;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Models.Criteria;
using Core.ServiceBus;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Api.Model.Shipment;
using Tradefact.Application.FreightMovements;
using Tradefact.Application.Models;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest;
using Tradefact.Application.Shipments.Commands;
using Tradefact.Application.Shipments.Queries.GetShipmentInfo;
using Tradefact.Application.Shipments.Queries.GetShipmentList;
using Tradefact.Data;
using Tradfact.Api.Requests;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    public partial class PartnerController : OrganisationController
    {
        private readonly ILogger<PartnerController> _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly ITradefactActivityService _tradefactActivityService;

        protected override OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.PARTNER;
        public PartnerController(TradefactDbContext context, IMediator mediator, ILocationService locationService, ILogger<PartnerController> logger, IServiceBusClient serviceBusClient, ITradefactActivityService tradefactActivityService) : base(context, mediator, tradefactActivityService, locationService)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
            _tradefactActivityService = tradefactActivityService;
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
                CompanyType = "ClientId",
                RequestingType = "ProviderId",
                InviteType = (int)InviteType.NewShipper,
                Paging = @params
            });

            if (clients == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(new ListResource<LogisticsClient>(clients));
        }
        
        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<QuotationRequestInfo>), Description = "Updated result")]
        [Route("QuotationRequest")]
        public async Task<IActionResult> GetQuotationRequests([FromQuery] PagedResultParameters @params, [FromQuery] QuotationSearchCriteria criteria)
        {
            IPagedList<QuotationRequestInfo> quotationRequests = await _mediator.Send(new GetQuotationRequestListQuery
            {
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
        public async Task<IActionResult> QuotationRequestCargoItemsById(string id, string freightMovementItemId, [FromQuery] PagedResultParameters @params)
        {
            Guid quoteRequestId = new Guid(id);

            IPagedList<CargoItemResource> cargoItemResources = await _mediator.Send(new GetQuotationRequestCargoItemsResourceQuery
            {
                QuotationRequestId = quoteRequestId,
                OrganisationId = this.OrganisationId,
                OrganisationType = this.OrganisationType,
                Paging = @params,
                ExistingFreightMovementItemId = freightMovementItemId
            });

            return Ok(new ListResource<CargoItemResource>(cargoItemResources));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(QuotationRequestResource), Description = "Created result")]
        [Route("Quotation")]
        public async Task<IActionResult> CreateQuote([FromBody]QuotationCreateRequest request)
        {
            QuotationRequest quotation_request = await _context.QuotationRequests
                .Include(i => i.Partnership.Client)
                .Include(i=>i.Quotations)
                .FirstOrDefaultAsync(q => q.Id == request.QuotationRequestId && q.Partnership.ProviderId == this.OrganisationId);

            int revision = 1;
            if (quotation_request.Quotations.Any())
            {
               revision = quotation_request.Quotations.Max(m => m.Revision) + 1;
            }
            // Mark any existing quotations inactive
            foreach (var existing in quotation_request.Quotations.Where(q => q.IsActive))
            {
                existing.IsActive = false;
            }

            if (quotation_request == null)
            {
                return new NotFoundResult();
            }
            Quotation q = new Quotation();

            q = request.Adapt(q);
            q.Revision = 1;
            
            // Loadtype optional
            q.LoadType = request.LoadType.HasValue ? request.LoadType.GetValueOrDefault() : null;

            if (quotation_request.Quotations.Any())
            {
                q.Revision = quotation_request.Quotations.Max(m => m.Revision) + 1;
            }

            if (request.IsManualRoute)
            {
                List<RouteSchedule> processed_manual_routes = new List<RouteSchedule>();

                List<RouteSchedule> manual_routes = JsonConvert.DeserializeObject<List<RouteSchedule>>(q.Routes);
                foreach (RouteSchedule manual_route in manual_routes)
                {
                    if (!string.IsNullOrEmpty(manual_route.CarrierCode))
                    {
                        // Manual schedule need to supplement the data
                        Carrier c = _context.Carriers.Find(manual_route.CarrierCode);
                        manual_route.CarrierName = c.Name;
                    }
                    else
                    {
                        manual_route.CarrierName = manual_route.CarrierName ?? "N/A";
                    }

                    Location pol = _context.Locations.FirstOrDefault(q => q.LocCode == manual_route.PortOfLoading.LocationCode);
                    DateTime? etd = new DateTime();
                    if (pol != null)
                    {
                        etd = manual_route.PortOfLoading.ETD.HasValue ? manual_route.PortOfLoading.ETD : null;
                        manual_route.PortOfLoading = new WayPoint
                        {
                            LocationCode = pol.LocCode,
                            ETD = etd,
                            FullName = pol.Name,
                            Name = pol.Name
                        };
                    }

                    Location pod = _context.Locations.FirstOrDefault(q => q.LocCode == manual_route.PortOfDischarge.LocationCode);
                    DateTime? eta = new DateTime();
                    if (pod != null)
                    {
                        eta = manual_route.PortOfDischarge.ETA.HasValue ? manual_route.PortOfDischarge.ETA : null;
                        manual_route.PortOfDischarge = new WayPoint
                        {
                            LocationCode = pod.LocCode,
                            ETA = eta,
                            FullName = pod.Name,
                            Name = pod.Name
                        };
                    }
                    
                    if (eta != null && etd != null)
                    {
                        manual_route.TransitTimeDays = ((DateTime)eta - (DateTime)etd).Days;
                        manual_route.TransitTimeHours = ((DateTime)eta - (DateTime)etd).Hours;
                        manual_route.TransitTimeMinutes = ((DateTime)eta - (DateTime)etd).Minutes;
                    }
                    
                    processed_manual_routes.Add(manual_route);
                }
                q.Routes = JsonConvert.SerializeObject(processed_manual_routes);
            }
            q.CurrencyId = request.CurrencyCode;

            q.CreationDate = q.IssueDate = DateTime.Now;
            q.Reference = $"Q-{this.GenerateReference()}";

            q.BaseCurrency = new Core.Models.Total() { CurrencyId = request.CurrencyCode, DiscountAmount = 0, NetAmount = 0, TaxAmount = 0, TotalAmount = 0 };
            q.TotalQuantity = 0;
            foreach (OriginCharge c in request.OriginCharges.Select((item, index) => new OriginCharge(item.Charge, item.Qty, item.Rate, item.Tax, 0, item.Margin, item.CurrencyCode ?? request.CurrencyCode, 1, index+1)))
            {
                q.OriginCharges.Add(c);
                q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                q.TotalQuantity += c.Quantity;
            }
            foreach (FreightCharge c in request.FreightCharges.Select((item, index) => new FreightCharge(item.Charge, item.Qty, item.Rate, item.Tax, 0, item.Margin, item.CurrencyCode ?? request.CurrencyCode, 1, index + 1)))
            {
                q.FreightCharges.Add(c);
                q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                q.TotalQuantity += c.Quantity;
            }
            foreach (DestinationCharge c in request.DestinationCharges.Select((item, index) => new DestinationCharge(item.Charge, item.Qty, item.Rate, item.Tax, 0, item.Margin, item.CurrencyCode ?? request.CurrencyCode, 1, index + 1)))
            {
                q.DestinationCharges.Add(c);
                q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                q.TotalQuantity += c.Quantity;
            }
            foreach (AdditionalCharge c in request.AdditionalCharges.Select((item, index) => new AdditionalCharge(item.Charge, item.Qty, item.Rate, item.Tax, 0, item.Margin, item.CurrencyCode ?? request.CurrencyCode, 1, index + 1)))
            {
                q.AdditionalCharges.Add(c);
                q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                q.TotalQuantity += c.Quantity;
            }
            q.Total = new Core.Models.Total() { CurrencyId = request.CurrencyCode, DiscountAmount = q.BaseCurrency.DiscountAmount, NetAmount = q.BaseCurrency.NetAmount, TaxAmount = q.BaseCurrency.TaxAmount, TotalAmount = q.BaseCurrency.TotalAmount };
            q.ExpiryDate = request.ValidUntil;

            q.PaymentTermsDays = request.PaymentTermsDays;
            q.ValidForDays = request.ValidForDays;
            q.Margin = request.Margin;
            q.TaxRate = request.TaxRate;

            q.Booked = false;
            q.FreightMovementId = quotation_request.FreightMovementId;

            quotation_request.State = QuotationStateEnum.READY;

            quotation_request.Quotations.Add(q);
            _ = await _context.SaveChangesAsync();

            await _tradefactActivityService.TrackEvent(User, this.OrganisationId, "quote_created", new TrackWith { Segment = true, Tradefact = false }, new EventProps { Segment = request });

            return new OkObjectResult(quotation_request.Adapt<QuotationRequestResource>());
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<ShipmentInfo>), Description = "Updated result")]
        [Route("Shipments")]
        public async Task<IActionResult> GetShipments([FromQuery] PagedResultParameters @params, [FromQuery] ShipmentSearchCriteria criteria)
        {
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
            ShipmentInfo s = await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId });
            QuotationRequestResource qr = await _mediator.Send(new GetQuotationRequestResourceQuery { QuotationRequestId = s.QuotationRequestId, OrganisationId = this.OrganisationId, OrganisationType = this.OrganisationType });
            s.FreightMovement = qr.FreightMovement;
            s.Quotation = qr.Quotation;

            return new OkObjectResult(s);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Route("Shipments/AmendSchedule")]
        public async Task<IActionResult> AmendShipmentSchedule([FromBody] AmendShipmentScheduleRequest request)
        {
            var cmdRescheduleShipment = new AmendShipmentScheduleCommand {
                OrganisationType = this.OrganisationType,
                OrganisationId = this.OrganisationId,
                ShipmentId = request.ShipmentId,
                GoodsReady = request.GoodsReady,
                IsManualRoute = request.IsManualRoute,

                DeparturelDate = request.DeparturelDate,
                ArrivalDate = request.ArrivalDate,
                Charge = request.Charge,
                Route = request.Route,

                IsReSchedule = request.IsReSchedule
            };
                
               // (this.OrganisationId, this.OrganisationType, request.QuotationRequestId, request.Route);
            _logger.LogInformation(
                "----- Sending command: Amend Shipment Schedule - {IdProperty}", cmdRescheduleShipment.ShipmentId);
            Guid shipmentId = await _mediator.Send(cmdRescheduleShipment);
            
            var shipment = _context.Shipments.FirstOrDefault(x => x.Id == cmdRescheduleShipment.ShipmentId);
            var fm = _context.FreightMovements.FirstOrDefault(x => x.Id == shipment.FreightMovementId);
            var user = _context.Users.FirstOrDefault(x => x.Email == shipment.CreatedByUser);
            var tagList = shipment.Tags;

            SendMessageToServiceBus(fm, user, tagList);


            var orgId = user.OrganisationId;
            var adminList = _context.Users.Where(x => x.OrganisationId == orgId && x.IsAdmin).ToList();

            if (adminList.Contains(user)) adminList.Remove(user);

            foreach (var admin in adminList)
            {
                SendMessageToServiceBus(fm, admin, tagList);
            }

            return new OkObjectResult(new { id = shipmentId });
        }



        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/AssignCollectionDate")]
        public async Task<IActionResult> ShipmentAssignCollectionDate([FromBody] AssignCollectionDateRequest request)
        {
            var command = new AssignShipmentCollectionDateCommand { Id = request.ShipmentId, CollectionDate = request.CollectionDate };
            var id = await _mediator.Send(command);

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/RecordCollection")]
        public async Task<IActionResult> ShipmentRecordCollection([FromBody] RecordCollectionRequest request)
        {
            var command = new RecordCollectedCommand { Id = request.ShipmentId, Collected = request.Collected };
            var id = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "shipment_marked_collected",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.SHIPMENT, CustomDescription = "In transit to port" } }
            );

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/TransitInformation")]
        public async Task<IActionResult> TransitInformation(Guid shipmentid)
        {
            return new OkObjectResult(await _mediator.Send(new GetShipmentTransitInfoQuery { ShipmentId = shipmentid }));
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/AddTransitInformation")]
        public async Task<IActionResult> AddTransitInformation([FromBody] AssignTrackingInformationRequest request)
        {
            var command = new AssignTransitInformationCommand { Id = request.ShipmentId, Carrier = request.Carrier, BOLNumber = request.BolNumber, ContainerIds = request.ContainerIds, Vessel = request.Vessel, VesselIMO = request.VesselIMO };
            var id = await _mediator.Send(command);

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [DisableFormValueModelBinding]
        [Route("Shipments/AssignBOLNumber")]
        public async Task<IActionResult> Shipment([FromBody] AssignShipmentBOLRequest request)
        {
            var command = new AssignTransitInformationCommand { Id = request.ShipmentId, BOLNumber = request.BolNumber, IsBOLNumberOnly = true };
            var id = await _mediator.Send(command);

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [DisableFormValueModelBinding]
        [Route("Shipments/AssignAWBNumber")]
        public async Task<IActionResult> AssignAWBNumber([FromBody] AssignAWBNumberRequest request)
        {
            string awb = $"{request.AWBPrefix}-{request.AWBSerialNo}";
            var command = new AssignTransitInformationCommand { Id = new Guid(request.ShipmentId), BOLNumber = awb, IsBOLNumberOnly = true };
            var id = await _mediator.Send(command);

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/RecordDepartedPOL")]
        public async Task<IActionResult> RecordInTransit([FromBody] RecordDepartedPOLRequest request)
        {
            var command = new RecordShipmentDepartedPOLCommand { Id = request.ShipmentId, DepartedDatePOL = request.DepartureDatePOL  };
            var id = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "shipment_confirm_departed",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.SHIPMENT, CustomDescription = "Shipping" } }
            );

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/RecordArrivedPOD")]
        public async Task<IActionResult> RecordArrivedPOD([FromBody] RecordArrivedPODRequest request)
        {
            var command = new RecordShipmentArrivedPODCommand { Id = request.ShipmentId, ArrivalDatePOD = request.ArrivalDatePOD };
            var id = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "shipment_confirm_arrival",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.SHIPMENT, CustomDescription = "Pending customs clearance" } }
            );

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/RecordIssueAtCustoms")]
        public async Task<IActionResult> RecordIssueAtCustoms([FromBody] RecordIssueAtCustomsRequest request)
        {
            var command = new RecordIssueAtCustomsCommand { Id = request.ShipmentId, IssueAtCustomsDate = request.IssueAtCustomsDate };
            var id = await _mediator.Send(command);

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/RecordClearedCustoms")]
        public async Task<IActionResult> RecordClearedCustoms([FromBody] RecordClearedCustomsRequest request)
        {
            var command = new RecordClearedCustomsCommand { Id = request.ShipmentId, ClearedCustomsDate = request.ClearedCustomsDate };
            var id = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "shipment_customs_cleared",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.SHIPMENT, CustomDescription = "In transit to destination" } }
            );

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/AssignEstimatedDeliveryDate")]
        public async Task<IActionResult> ShipmentAssignEstimatedDeliveryDate([FromBody] RecordEstimatedDeliveryDateRequest request)
        {
            var command = new AssignShipmentEstimatedDeliveryDateCommand { Id = request.ShipmentId, EstimatedDeliveryDate = request.EstimatedDeliveryDate };
            var id = await _mediator.Send(command);

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ShipmentInfo), Description = "Updated result")]
        [Route("Shipments/MarkDelivered")]
        public async Task<IActionResult> RecordDelivered([FromBody] RecordDeliveryCompleteRequest request)
        {
            var command = new RecordDeliveryCompleteCommand { Id = request.ShipmentId, Delivered = request.Delivered, DeliveryCompleteDate = request.DeliveryDate };
            var id = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "shipment_marked_delivered",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.SHIPMENT, CustomDescription = "Delivered" } }
            );

            return new OkObjectResult(await _mediator.Send(new GetShipmentInfoQuery { ShipmentId = id, ProviderId = this.OrganisationId }));
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
                Paging = @params,
                Search = search
            });
            return this.HandleSuccessResponse(new ListResource<CargoItemResource>(result));
        }

        private List<QuotationChargeItem> BuildChargeItems()
        {
            List<QuotationChargeItem> retValue = new List<QuotationChargeItem>();

            return retValue;
        }

        private async void SendMessageToServiceBus(FreightMovement fm, ApplicationUser user, string tags)
        {
            string QueueName = "reschedulequeue";

            ServiceBusMessage<ShipmentRescheduledMail> msg = new ServiceBusMessage<ShipmentRescheduledMail>(new ShipmentRescheduledMail
            {
                Email = user.Email,
                PartnerName = user.GivenName,
                ShipmentName = fm.Name,
                TagList = tags,
                TradefactId = fm.Reference
            });

            await _serviceBusClient.Publish<ShipmentRescheduledMail>(msg, QueueName);
        }
    }
}