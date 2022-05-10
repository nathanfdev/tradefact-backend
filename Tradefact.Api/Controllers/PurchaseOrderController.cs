using Core.Common;
using Core.Dtos.PurchaseOrder;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Models.Criteria;
using Core.ServiceBus;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Api.Infrastructure.Helpers;
using Tradefact.Api.Model;
using Tradefact.Api.Model.PurchaseOrder.Requests;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Application.Models.Activity;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.PurchaseOrders;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Application.Schedules.Model;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : BaseApiController
    {
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly IServiceBusClient _serviceBusClient;
        protected readonly IMediator _mediator;
        private readonly SigningUrlHelper _signingUrlHelper;
        private readonly ITradefactActivityService _tradefactActivityService;
        private readonly AppSettings AppSettings;

        public PurchaseOrderController(TradefactDbContext context, IServiceBusClient serviceBusClient, IMediator mediator, ILogger<PurchaseOrderController> logger, IOptions<AppSettings> appSettings, SigningUrlHelper signingUrlHelper, ITradefactActivityService tradefactActivityService) : base(context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._signingUrlHelper = signingUrlHelper;
            _serviceBusClient = serviceBusClient;
            _tradefactActivityService = tradefactActivityService;
            this.AppSettings = appSettings.Value;
        }

        private async Task<PurchaseOrderResource> ByIdAsync(Guid id, bool includeItems = default, bool includeNotes = default)
        {
            PurchaseOrderResource po = await _mediator.Send(new GetPurchaseOrderByIdQuery
            {
                OrganisationId = this.OrganisationId,
                OrganisationType = this.OrganisationType,
                PurchaseOrderId = id,
                includeItems = includeItems
                
            });
            po.ViewerURL = _signingUrlHelper.GenerateSignedURL("POViewer", $"pid={po.Id.ToString()}");
            return po;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<PurchaseOrderResource>), Description = "Updated result")]
        public async Task<IActionResult> Get([FromQuery] PagedResultParameters @params, [FromQuery] PurchaseOrderSearchCriteria criteria)
        {
            if (criteria.View == PurchaseOrderViewEnum.WIDGET)
            {
                PurchaseOrderWidgetResource widgetOrders = await _mediator.Send(new GetPurchaseOrderWidgetQuery
                {
                    OrganisationId = this.OrganisationId
                });

                return new OkObjectResult(widgetOrders);
            }

            IPagedList<PurchaseOrderResource> orders = await _mediator.Send(new GetAllPurchaseOrdersQuery
            {
                OrganisationId = this.OrganisationId,
                OrganisationType = this.OrganisationType,
                Criteria = criteria,
                Paging = @params
            });
            return new OkObjectResult(new ListResource<PurchaseOrderResource>(orders));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<PurchaseOrderResource>), Description = "Updated result")]
        [Route("NoProductSearch")]
        public async Task<IActionResult> GetNoProductSearch([FromQuery] PagedResultParameters @params, [FromQuery] PurchaseOrderSearchCriteria criteria)
        {
            IPagedList<PurchaseOrderResource> orders = await _mediator.Send(new GetAllPurchaseOrdersNoProductsQuery
            {
                OrganisationId = this.OrganisationId,
                OrganisationType = this.OrganisationType,
                Criteria = criteria,
                Paging = @params
            });
            return new OkObjectResult(new ListResource<PurchaseOrderResource>(orders));
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, bool includeItems = default, bool includeNotes = false)
        {
            PurchaseOrderResource resource = await this.ByIdAsync(id, includeItems, includeNotes);


            return new OkObjectResult(resource);
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Created result")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request)
        {
            Guid id = await _mediator.Send(new PurchaseOrderCreateCommand(this.OrganisationId, request.SupplierId, request.CurrencyId, request.PONumber));
            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }


        [HttpPut]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] PurchaseOrderEditDto request, Guid id)
        {
            Guid po = await _mediator.Send(new PurchaseOrderUpdateCommand(this.OrganisationId, id, request));
            return new OkObjectResult(await this.ByIdAsync(po, false, false));
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Update result")]
        [Route("{id:guid}")]
        public virtual async Task<IActionResult> Patch([FromBody] JsonPatchDocument<PurchaseOrderEditDto> patchDoc, Guid id)
        {
            if (patchDoc != null)
            {
                var po = await _mediator.Send(new PurchaseOrderPatchCommand(this.OrganisationId, id, patchDoc));
                // var po = await _mediator.Send(new PurchaseOrderPatchCommand(this.OrganisationId, request));
                return new OkObjectResult(await this.ByIdAsync(id, false, false));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Deleted result")]
        [Route("{id:guid}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var po = await this.ByIdAsync(id, false, false);
            if (po != null && po.AvailableActions.CanDelete)
            {
                var command = new PurchaseOrderDeleteCommand(this.OrganisationId, id );
                var pid = await _mediator.Send(command);

                return new OkObjectResult(new { });
            }
            return new BadRequestResult();
        }

        // ------ Purchase Order Items

        private async Task<PurchaseOrderItemResource> ByLineIdAsync(Guid purchaseOrderId, Guid purchaseOrderItemId, bool includeNotes = default)
        {
            PurchaseOrderItemResource item = await _mediator.Send(new GetPurchaseOrderItemByIdQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = purchaseOrderId,
                PurchaseOrderItemId = purchaseOrderItemId
            });
            return item;
        }
        private async Task<List<PurchaseOrderItemResource>> ByLineIdsAsync(Guid purchaseOrderId, List<Guid> purchaseOrderItemIds, bool includeNotes = default)
        {
            List<PurchaseOrderItemResource> items = await _mediator.Send(new GetPurchaseOrderItemsByIdQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = purchaseOrderId,
                PurchaseOrderItemIds = purchaseOrderItemIds
            });
            return items;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<PurchaseOrderItemResource>), Description = "OK Result")]
        [Route("{id:guid}/Items")]
        public async Task<IActionResult> GetItems(Guid id)
        {
            List<PurchaseOrderItemResource> items = await _mediator.Send(new GetPurchaseOrderItemsQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id
            });
            return new OkObjectResult(items);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<QuotationRequestInfo>), Description = "OK result")]
        [Route("{id:guid}/Shipments")]
        public async Task<IActionResult> GetShipments(Guid id, [FromQuery] PurchaseOrderShipmentSearchCriteria criteria)
        {
            IPagedList<ShipmentInfo> shipments = await _mediator.Send(new GetPurchaseOrderShipmentListQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id,
                OrganisationType = this.OrganisationType,
                Criteria = criteria,
                Paging = new PagedResultParameters { PageNumber = 1, PageSize = int.MaxValue}
            });
            return new OkObjectResult(new ListResource<ShipmentInfo>(shipments));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<QuotationRequestInfo>), Description = "OK result")]
        [Route("{id:guid}/Quotes")]
        public async Task<IActionResult> GetQuotes(Guid id, [FromQuery] PurchaseOrderShipmentSearchCriteria criteria)
        {
            IPagedList<QuotationRequestInfo> quotations = await _mediator.Send(new GetPurchaseOrderActiveQuotationRequestListQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id,
                Criteria = criteria,
                OrganisationType = this.OrganisationType,
                Paging = new PagedResultParameters { PageNumber = 1, PageSize = int.MaxValue }
            });
            return new OkObjectResult(new ListResource<QuotationRequestInfo>(quotations));
        }


        [HttpPost, HttpGet]
        [SwaggerResponse("200", typeof(List<PurchaseOrderItemResource>), Description = "OK Result")]
        [Route("{id:guid}/csv")]
        public async Task<FileStreamResult> GetItemsCSV(Guid id)
        {
            List<PurchaseOrderItemResource> items = await _mediator.Send(new GetPurchaseOrderItemsQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id
            });

            var data = items.Select(s => new
            {
                Product = s.Product.Name,
                SKU = s.Product.SKU,
                Packing = s.Product.Packing,
                Quantity = s.OrderQuantity,
                Price = s.OrderPriceUnit
            });

            byte[] byteArray = Encoding.ASCII.GetBytes(WriteCsv(data));
            MemoryStream stream = new MemoryStream(byteArray);

            return new FileStreamResult(stream, "application/vnd.ms-excel")
            {
                FileDownloadName = "PurchaseOrderExport.csv"
            };
        }


        private string WriteCsv<T>(IEnumerable<T> data)
        {
            StringBuilder output = new StringBuilder();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            List<string> line = new List<string>();
            foreach (PropertyDescriptor prop in props)
            {
                line.Add(prop.DisplayName);
            }
            output.AppendLine(String.Join(',', line));


            foreach (T item in data)
            {
                line.Clear();
                foreach (PropertyDescriptor prop in props)
                {
                    line.Add(SanitiseData(prop.Converter.ConvertToString(
                         prop.GetValue(item))));
                }
                output.AppendLine(String.Join(',', line));
            }
            return output.ToString();
        }

        private string SanitiseData(string data)
        {
            return data.Replace(',', '/');
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "OK Result")]
        [Route("{id:guid}/Items/{itemid:guid}")]
        public async Task<IActionResult> GetItem(Guid id, Guid itemid)
        {
            return new OkObjectResult(await this.ByLineIdAsync(id, itemid));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Created result")]
        [Route("{id:guid}/Items")]
        public async Task<IActionResult> CreateItem([FromBody] PurchaseOrderItemCreateDto request, Guid id)
        {
            List<PurchaseOrderItemEditDto> insert = new List<PurchaseOrderItemEditDto> { new PurchaseOrderItemEditDto { ProductId = request.ProductId, OrderQuantity = 0 } };
            PurchaseOrderItemCreateCommand cmd = new PurchaseOrderItemCreateCommand(this.OrganisationId, id, insert);

            List<Guid> newItemIds = await _mediator.Send(cmd);
            return new OkObjectResult(await this.ByLineIdAsync(id, newItemIds[0], false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(List<PurchaseOrderItemResource>), Description = "Created result")]
        [Route("{id:guid}/Items/list")]
        public async Task<IActionResult> CreateManyItems([FromBody] List<PurchaseOrderItemEditDto> request, Guid id, bool AllProducts = false, bool LinkedOnly = false, string SupplierId = null)
        {
            if (AllProducts)
            {
                IPagedList<ProductResource> allProducts = await _mediator.Send(new GetProductOrderQuantityListQuery
                {
                    OrganisationId = this.OrganisationId,
                    Paging = new PagedResultParameters { NoPage = true },
                    SupplierId = LinkedOnly && !string.IsNullOrEmpty(SupplierId) ? new Guid(SupplierId) : null
                });
                
                request = allProducts.Select(p => new PurchaseOrderItemEditDto 
                {
                    ProductId = new Guid(p.Id),
                    OrderPriceUnit = string.IsNullOrEmpty(SupplierId) ? null: 
                        p.Suppliers.FirstOrDefault(q => q.SupplierId == new Guid(SupplierId))?.Price,
                    SupplierReference = string.IsNullOrEmpty(SupplierId) ? null :
                        p.Suppliers.FirstOrDefault(q => q.SupplierId == new Guid(SupplierId))?.SupplierReference
                }).ToList();
            }

            PurchaseOrderItemCreateCommand cmd = new PurchaseOrderItemCreateCommand(this.OrganisationId, id, request);
            List<Guid> newItemIds = await _mediator.Send(cmd);

            return new OkObjectResult(await this.ByLineIdsAsync(id, newItemIds, false));
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Updated result")]
        [Route("{id:guid}/Items/{itemid:guid}")]
        public async Task<IActionResult> PutItem([FromBody] PurchaseOrderItemEditDto request, Guid id, Guid itemid)
        {
            Guid itemId = await _mediator.Send(new PurchaseOrderItemUpdateCommand(this.OrganisationId, id, itemid, request));
            return new OkObjectResult(await this.ByLineIdAsync(id, itemId, false));
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Update result")]
        [Route("{id:guid}/Items/{itemid:guid}")]
        public virtual async Task<IActionResult> PatchItem([FromBody] JsonPatchDocument<PurchaseOrderItemEditDto> patchDoc, Guid id, Guid itemid)
        {
            if (patchDoc != null)
            {
                var itemId = await _mediator.Send(new PurchaseOrderItemPatchCommand(this.OrganisationId, id, itemid, patchDoc));
                return new OkObjectResult(await this.ByLineIdAsync(id, itemId, false));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Update result")]
        [Route("{id:guid}/Items/{itemid:guid}")]
        public virtual async Task<IActionResult> DeleteItem(Guid id, Guid itemid)
        {
            var itemId = await _mediator.Send(new PurchaseOrderItemDeleteCommand(this.OrganisationId, id, itemid));
            return new OkObjectResult(await this.ByLineIdAsync(id, itemId, false));
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Update result")]
        [Route("{id:guid}/items")]
        public virtual async Task<IActionResult> DeleteAllItems(Guid id)
        {
            var itemId = await _mediator.Send(new PurchaseOrderItemDeleteAllCommand(this.OrganisationId, id));

            PurchaseOrderResource resource = await this.ByIdAsync(id, false, false);
            return new OkObjectResult(resource);
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderItemResource), Description = "Update result")]
        [Route("{id:guid}/items/copy/{originalid}")]
        public virtual async Task<IActionResult> CopyAllItems(Guid id, Guid originalid)
        {
            var itemId = await _mediator.Send(new PurchaseOrderItemCopyAllCommand(this.OrganisationId, id, originalid));

            PurchaseOrderResource resource = await this.ByIdAsync(id, false, false);
            return new OkObjectResult(resource);
        }

        [HttpPost]
        [Route("{id:guid}/requestupdate")]
        public async Task<IActionResult> RequestUpdate(Guid id)
        {
            PurchaseOrder po = await _context.PurchaseOrders.FindAsync(id);
            Organisation org = await _context.Organisations.FindAsync(this.OrganisationId);
            Organisation supplier = await _context.Organisations.FindAsync(po.SupplierId);

            if (supplier.ParentId == null)
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string linkenv = "";
                if (env == "DEVELOPMENT" || env == "azure_dev") linkenv = "dev.app";
                else if (env == "azure_prd") linkenv = "app";

                StringBuilder link = new StringBuilder();
                link.Append($"https://{linkenv}.tradefact.com/#/order/{po.Id}");
                SendMessageToServiceBusOrderUpdateRequest(supplier.ContactEmail, supplier.ContactName, org.Name, link.ToString());
            }
            else
            {
                SendMessageToServiceBusOrderUpdateRequest(supplier.ContactEmail, supplier.ContactName, org.Name, "");
            }
            return new OkObjectResult("");
        }

        // ---------------------------------------------------------------------------------------------------------------------------------
        // Charge Items


        private async Task<PurchaseOrderAdditionalChargeItemResource> ChargeItemByIdAsync(Guid purchaseOrderId, Guid purchaseOrderAdditionalChargeItemId)
        {
            PurchaseOrderAdditionalChargeItemResource item = await _mediator.Send(new GetPurchaseOrderChargeItemByIdQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = purchaseOrderId,
                PurchaseOrderChargeItemId = purchaseOrderAdditionalChargeItemId
            });

            return item;
        }

        // ---------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        [SwaggerResponse("200", typeof(List<PurchaseOrderAdditionalChargeItemResource>), Description = "OK Result")]
        [Route("{id:guid}/AdditionalCharges")]
        public async Task<IActionResult> GetAdditionalCharges(Guid id)
        {
            List<PurchaseOrderAdditionalChargeItemResource> additionalChargeItemsitems = await _mediator.Send(new GetPurchaseOrderChargeItemsQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id
            });
            return new OkObjectResult(additionalChargeItemsitems);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(PurchaseOrderAdditionalChargeItemResource), Description = "OK Result")]
        [Route("{id:guid}/AdditionalCharges/{itemid:guid}")]
        public async Task<IActionResult> GetAdditionalChargeItem(Guid id, Guid itemid)
        {
            return new OkObjectResult(await this.ChargeItemByIdAsync(id, Guid.NewGuid()));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderAdditionalChargeItemResource), Description = "Created result")]
        [Route("{id:guid}/AdditionalCharges")]
        public async Task<IActionResult> CreateAdditionalChargeItem([FromBody] CreatePurchaseOrderAdditionalChargeRequest request, Guid id)
        {
            PurchaseOrderChargeItemCreateCommand cmd = new PurchaseOrderChargeItemCreateCommand(this.OrganisationId, id, request);
            Guid newItemId = await _mediator.Send(cmd);

            return new OkObjectResult(await this.ChargeItemByIdAsync(id, newItemId));
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(PurchaseOrderAdditionalChargeItemResource), Description = "Updated result")]
        [Route("{id:guid}/AdditionalCharges/{itemid:guid}")]
        public async Task<IActionResult> PutAdditionalChargeItem([FromBody] UpdatePurchaseOrderAdditionalChargeRequest request, Guid id, Guid itemid)
        {
            PurchaseOrderChargeItemUpdateCommand cmd = new PurchaseOrderChargeItemUpdateCommand(this.OrganisationId, id, itemid, request);
            Guid itemId = await _mediator.Send(cmd);
            return new OkObjectResult(await this.ChargeItemByIdAsync(id, itemId));
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(PurchaseOrderAdditionalChargeItemResource), Description = "Update result")]
        [Route("{id:guid}/AdditionalCharges/{itemid:guid}")]
        public virtual async Task<IActionResult> PatchAdditionalChargeItem([FromBody] JsonPatchDocument<UpdatePurchaseOrderAdditionalChargeRequest> patchDoc, Guid id, Guid itemid)
        {
            if (patchDoc != null)
            {
                var itemId = await _mediator.Send(new PurchaseOrderChargeItemPatchCommand(this.OrganisationId, id, itemid, patchDoc));
                return new OkObjectResult(await this.ChargeItemByIdAsync(id, itemId));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(PurchaseOrderAdditionalChargeItemResource), Description = "Update result")]
        [Route("{id:guid}/AdditionalCharges/{itemid:guid}")]
        public virtual async Task<IActionResult> DeleteAdditionalChargeItem(Guid id, Guid itemid)
        {
            var itemId = await _mediator.Send(new PurchaseOrderChargeItemDeleteCommand(this.OrganisationId, id, itemid));
            return new OkObjectResult(await this.ChargeItemByIdAsync(id, itemId));
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitPurchaseOrderRequest request, Guid id)
        {
            var command = new RecordSubmittedCommand { PurchaseOrderId = id, SubmissionDate = request.SubmissionDate.HasValue ? request.SubmissionDate.GetValueOrDefault() : DateTime.Now, Submitted = request.Submitted };
            _ = await _mediator.Send(command);

            List<POContactRequest> poContactRequest = await GenerateContactRegistrationLinks(request, id);

            var send_command = new SendPurchaseOrderEmailCommand { PurchaseOrderId = id, Contacts = poContactRequest };
            _ = await _mediator.Send(send_command);

            await _tradefactActivityService.TrackEvent(
                User, 
                this.OrganisationId,
                "purchase_order_sent", 
                new TrackWith { Segment = true, Tradefact = true },
                new EventProps { Segment = request, Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            );

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Resend")]
        public async Task<IActionResult> Resend([FromBody] ResendPurchaseOrderRequest request, Guid id)
        {
            if (request.Contacts?.Count > 0)
            {
                SubmitPurchaseOrderRequest resend_request = new SubmitPurchaseOrderRequest { Contacts = request.Contacts };

                List<POContactRequest> poContactRequest = await GenerateContactRegistrationLinks(resend_request, id);

                var send_command = new SendPurchaseOrderEmailCommand { PurchaseOrderId = id, Contacts = poContactRequest };
                _ = await _mediator.Send(send_command);

                return new OkObjectResult(await this.ByIdAsync(id, false, false));
            }
            return new BadRequestResult();
        }


        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Reset")]
        public async Task<IActionResult> Reset(Guid id)
        {
            var po = await this.ByIdAsync(id, false, false);
            if (po != null && po.AvailableActions.Reset)
            {
                var command = new ResetPurchaseOrderCommand { PurchaseOrderId = id, ResetDate = DateTime.Now, Reset = true };
                var pid = await _mediator.Send(command);

                return new OkObjectResult(new { });
            }
            return new BadRequestResult();

        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Accept")]
        public async Task<IActionResult> Accept([FromBody] AcceptPurchaseOrderRequest request, Guid id)
        {
            var command = new RecordAcceptedCommand { PurchaseOrderId = id, AcceptanceDate = request.AcceptanceDate.HasValue ? request.AcceptanceDate.GetValueOrDefault() : DateTime.Now, Accepted = request.Accepted };
            var pid = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "purchase_order_status_change",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            );

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectPurchaseOrderRequest request, Guid id)
        {
            var command = new RecordRejectedCommand { PurchaseOrderId = id, RejectionDate = request.RejectionDate.HasValue ? request.RejectionDate.GetValueOrDefault() : DateTime.Now, Rejected = request.Rejected, Notes = request.Notes };
            var pid = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "purchase_order_status_change",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            );

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Draft")]
        public async Task<IActionResult> Draft(Guid id)
        {
            var command = new RecordDraftCommand { PurchaseOrderId = id };
            var pid = await _mediator.Send(command);

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/InProduction")]
        public async Task<IActionResult> InProduction([FromBody] UpdatePurchaseOrderProductionStateRequest request, Guid id)
        {
            var command = new RecordInProductionCommand { PurchaseOrderId = id, ProductionDate = request.ProductionStateChangeDate.HasValue ? request.ProductionStateChangeDate.GetValueOrDefault() : DateTime.Now, InProduction = request.InProduction };
            var pid = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "purchase_order_status_change",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            ); 

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/PreShipment")]
        public async Task<IActionResult> PreProduction([FromBody] UpdatePurchaseOrderPreShippingStateRequest request, Guid id)
        {
            var command = new RecordPreShipmentCommand { PurchaseOrderId = id, PreShipmentDate = request.PreProductionStateChangeDate.HasValue ? request.PreProductionStateChangeDate.GetValueOrDefault() : DateTime.Now, PreShipment = request.PreProduction };
            var pid = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "purchase_order_status_change",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            );

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Shipped")]
        public async Task<IActionResult> Shipped([FromBody] UpdatePurchaseOrderShippingStateRequest request, Guid id)
        {
            var command = new RecordShippingCommand { PurchaseOrderId = id, ShippedDate = request.ShippingStateChangeDate.HasValue ? request.ShippingStateChangeDate.GetValueOrDefault() : DateTime.Now, Shipping = request.Shipping };
            var pid = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "purchase_order_status_change",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            );

            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Updated result")]
        [Route("{id:guid}/Complete")]
        public async Task<IActionResult> Complete([FromBody] CompletePurchaseOrderRequest request, Guid id)
        {
            var command = new RecordCompleteCommand { PurchaseOrderId = id, CompletionDate = request.CompletionDate.HasValue ? request.CompletionDate.GetValueOrDefault() : DateTime.Now, Complete = request.Complete };
            var pid = await _mediator.Send(command);

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "purchase_order_status_change",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.STATUS, Entity = ActivityEntityTypeEnum.PURCHASEORDER } }
            );


            return new OkObjectResult(await this.ByIdAsync(id, false, false));
        }

        private async Task<List<PurchaseOrderProductDocumentResource>> GetAttachedProductDocsAsync(Guid id, Guid productid, bool attachedOnly = false, ProductDocumentType docType = ProductDocumentType.All)
        {
            return await _mediator.Send(new GetPurchaseOrderProductDocumentsQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id,
                ProductId = productid,
                AttachedOnly = attachedOnly,
                DocumentType = docType
            });
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<PurchaseOrderProductDocumentResource>), Description = "OK Result")]
        [Route("{id:guid}/product/documents")]
        public async Task<IActionResult> GetProductDocuments(Guid id, ProductDocumentType doctype = ProductDocumentType.All)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string userEmail = identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault();

            List<PurchaseOrderProductDocumentResource> docs = await _mediator.Send(new GetPurchaseOrderDocumentsQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id,
                DocumentType = doctype,
                UserEmail = userEmail
            });

            return new OkObjectResult(docs);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<PurchaseOrderProductDocumentResource>), Description = "OK Result")]
        [Route("{id:guid}/product/{productid:guid}/documents")]
        public async Task<IActionResult> GetAttachedProductDocuments(Guid id, Guid productid, bool attachedonly = false, ProductDocumentType doctype = ProductDocumentType.All)
        {
            return new OkObjectResult(await GetAttachedProductDocsAsync(id, productid, attachedonly, doctype));
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(List<PurchaseOrderProductDocumentResource>), Description = "Created result")]
        [Route("{id:guid}/product/{productid:guid}/document/{documentid:guid}")]
        public async Task<IActionResult> AttachProductDocument(Guid id, Guid productid, Guid documentid)
        {

            this._context.PurchaseOrderAttachedProductDocuments.Add(new Core.Models.PurchaseOrderAttachedProductDocument { PurchaseOrderId = id, PurchaseOrderProductId = productid, DocumentId = documentid, IsActive = true });
            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(await GetAttachedProductDocsAsync(id, productid));
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(List<PurchaseOrderProductDocumentResource>), Description = "Update result")]
        [Route("{id:guid}/product/{productid:guid}/document/{documentid:guid}")]
        public virtual async Task<IActionResult> DeleteAttachedProductDocument(Guid id, Guid productid, Guid documentid)
        {
            var attched_doc = this._context.PurchaseOrderAttachedProductDocuments.FirstOrDefault(q=>q.PurchaseOrderId == id && q.PurchaseOrderProductId == productid && q.DocumentId == documentid);
            if ( attched_doc == null)
            {
                return new NoContentResult();
            }
            this._context.PurchaseOrderAttachedProductDocuments.Remove(attched_doc);
            _ = await _context.SaveChangesAsync();
            return new OkObjectResult(await GetAttachedProductDocsAsync(id, productid));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<PurchaseOrderEventResource>), Description = "OK Result")]
        [Route("{id:guid}/timeline")]
        public async Task<IActionResult> GetTimeline([FromQuery] PagedResultParameters @params, Guid id)
        {
            var timeline = await _mediator.Send(new GetPurchaseOrderTimelineQuery
            {
                PurchaseOrderId = id,
                Paging = @params
            });

            return new OkObjectResult(new ListResource<PurchaseOrderEventResource>(timeline));
        }
        
        [HttpPost]
        [SwaggerResponse("200", typeof(PurchaseOrderResource), Description = "Created result")]
        [Route("{id:guid}/shipping/quote")]
        public async Task<IActionResult> ShipmentQuotation(Guid id)
        {
            var command = new PurchaseOrderRequestShippingQuotationCommand(this.OrganisationId, id);
            var pid = await _mediator.Send(command);
            
            return new OkObjectResult(new { });
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(List<PurchaseOrderScheduleLineResource>), Description = "OK Result")]
        [Route("{id:guid}/Schedule")]
        public async Task<IActionResult> GetSchedule(Guid id, [FromQuery] PagedResultParameters @params, [FromQuery] SchedulesSearchCriteria criteria)
        {
            IPagedList<PurchaseOrderScheduleLineResource> schedule = await _mediator.Send(new GetPurchaseOrderSchedulesByPurchaseOrderIdQuery
            {
                OrganisationId = this.OrganisationId,
                PurchaseOrderId = id,
                Paging = @params
            });
            return new OkObjectResult(new ListResource<PurchaseOrderScheduleLineResource>(schedule));
        }

        private async void SendMessageToServiceBusOrderUpdateRequest(string email, string name, string organisationName, string link)
        {

            string QueueName = "orderupdaterequest";

            ServiceBusMessage<OrderUpdate> msg = new ServiceBusMessage<OrderUpdate>(new OrderUpdate
            {
                Email = email,
                FirstName = name,
                OrganisationName = organisationName,
                Link = link
            });

            await _serviceBusClient.Publish<OrderUpdate>(msg, QueueName);
        }

        private async Task<List<POContactRequest>> GenerateContactRegistrationLinks(SubmitPurchaseOrderRequest request, Guid id)
        {
            PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Shipment), id);
            }

            var org = await _context.Organisations
                    .Include(i => i.Contacts).ThenInclude(contact => contact.Email)
                    .SingleOrDefaultAsync(q => q.Id == entity.SupplierId);

            B2BConnection connection = await _context.B2BConnections
                    .Include(i => i.Contacts).ThenInclude(i => i.Email)
                    .SingleOrDefaultAsync(q => q.LinkedOrganisationId == entity.SupplierId);

            List<POContactRequest> poContactRequest = new List<POContactRequest>();

            // Supplier has contacts
            if (org.Contacts != null && org.Contacts.Count > 0)
            {
                foreach (Guid contactId in request.Contacts)
                {
                    OrganisationContact contact = org.Contacts.FirstOrDefault(q => q.Id == contactId);
                    if (contact != null && contact.Email.Count > 0)
                    {
                        foreach (var email in contact.Email)
                        {
                            if (email != null && !String.IsNullOrEmpty(email.Email) && email.IsActive)
                            {
                                Guid invitationId = Guid.NewGuid();
                                string token = BuildIdToken(org.Name, email.Email, invitationId.ToString(), InviteType.NewShipper.ToString());
                                string registrationLink = BuildRegistrationUrl(token);

                                poContactRequest.Add(new POContactRequest { Id = contactId, RegistrationLink = registrationLink });
                                if (email.IsDefault) break;
                            }
                        }
                    }
                }
            } else if (connection.Contacts != null && connection.Contacts.Count > 0)
            {
                Organisation connectedOrg = _context.Organisations.FirstOrDefault(o => o.Id == connection.LinkedOrganisationId);

                if (connectedOrg is null)
                {
                    return poContactRequest;
                }
                foreach (Guid contactId in request.Contacts)
                {
                    ConnectionContact contact = connection.Contacts.FirstOrDefault(q => q.Id == contactId);
                    if (contact != null && contact.Email.Count > 0)
                    {
                        foreach (var email in contact.Email)
                        {
                            if (email != null && !String.IsNullOrEmpty(email.Email) && email.IsActive)
                            {
                                Guid invitationId = Guid.NewGuid();
                                string token = BuildIdToken(connectedOrg.Name, email.Email, invitationId.ToString(), InviteType.NewShipper.ToString());
                                string registrationLink = BuildRegistrationUrl(token);

                                poContactRequest.Add(new POContactRequest { Id = contactId, RegistrationLink = registrationLink });
                                if (email.IsDefault) break;
                            }
                        }
                    }
                }
            }

            return poContactRequest;
        }

        private string BuildIdToken(string companyname, string emailaddress, string uniqueid, string invitationType)
        {
            string issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";

            var securityKey = Encoding.UTF8.GetBytes("2VK62QTn0m1hMcn0DQ3TRADArF4ct6yIiSvYgdRwjZtU5QhI=");

            var signingKey = new SymmetricSecurityKey(securityKey);
            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim("invite.email", emailaddress, System.Security.Claims.ClaimValueTypes.String, issuer));
            claims.Add(new System.Security.Claims.Claim("invite.id", uniqueid, System.Security.Claims.ClaimValueTypes.String, issuer));
            claims.Add(new System.Security.Claims.Claim("invite.company", companyname, System.Security.Claims.ClaimValueTypes.String, issuer));
            claims.Add(new System.Security.Claims.Claim("invite.type", invitationType, System.Security.Claims.ClaimValueTypes.String, issuer));

            // var signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512);

            // Create the token
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer,
                    this.AppSettings.B2CClientId,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    signingCredentials);

            // Get the representation of the signed token
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            return jwtHandler.WriteToken(token);
        }

        private string BuildRegistrationUrl(string token)
        {
            string nonce = Guid.NewGuid().ToString("n");

            return string.Format(this.AppSettings.B2CSignUpUrl,
                    this.AppSettings.B2CTenant,
                    this.AppSettings.B2CPolicy,
                    this.AppSettings.B2CClientId,
                    Uri.EscapeDataString(this.AppSettings.B2CRedirectUri),
                    nonce) + "&id_token_hint=" + token;
        }
    }
}
