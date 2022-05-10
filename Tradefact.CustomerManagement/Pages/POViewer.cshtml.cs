using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;
using Tradefact.Portal.Infrastructure.Filters;

namespace Tradefact.Portal.Pages
{
    [ValidateSharedURLSignature]
    public class POViewerModel : PageModel
    {

        [FromQuery(Name = "pid")]
        public string pid { get; set; }

        [FromQuery(Name = "sig")]
        public string sig { get; set; }

        [BindProperty]
        [HiddenInput]
        public string ActivePurchaseOrderId { get; set; }


        public bool ShowActions { get; set; } = false;

        private readonly TradefactDbContext _context;
        private readonly ILogger<POViewerModel> _logger;
        protected readonly IMediator _mediator;

        public PurchaseOrderResource PO { get; set; }

        public POViewerModel(TradefactDbContext context, IMediator mediator, ILogger<POViewerModel> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Message { get; private set; } = "PageModel in C#";
        public List<SelectListItem> ActionsAvailable { get; set; }

        [BindProperty] 
        public string SelectedAction { get; set; }

        public async Task OnGet()
        {
            await this.PopulatePO();
        }

        public async Task<IActionResult> OnPost()
        {
            Guid purchase_order_id = await this.UpdatePOStatus(this.ActivePurchaseOrderId);

            return new RedirectToPageResult("POAcknowledged")
            {
                RouteValues = new RouteValueDictionary
                    {
                        { "pid", pid.ToString() },
                        { "sig", sig.ToString() }
                    }
            };
        }

        private async Task<Guid> UpdatePOStatus(string order_id)
        {
            Guid id = Guid.Parse(this.ActivePurchaseOrderId);
            var p = await _context.PurchaseOrders.Where(q => q.Id == id).Select(s => new { PurchaseOrderId = s.Id, OrganisationId = s.CompanyId }).FirstOrDefaultAsync();

            switch (this.SelectedAction)
            {
                case "1-Accept":  // Accept
                    return await this.AcceptPO(p.PurchaseOrderId);

                case "2-Reject":  //Reject
                    return await this.RejectPO(p.PurchaseOrderId);

                case "3-InProd":  // In Production
                    return await this.SetInProduction(p.PurchaseOrderId);

                case "4-PreShip":  // Pre-Shipping
                    return await this.SetPreShipped(p.PurchaseOrderId);

                case "5-Shipping":  // Shipping
                    return await this.SetShipped(p.PurchaseOrderId);

                case "6-Complete":  // Complete
                    return await this.SetComplete(p.PurchaseOrderId);

                default:
                    return id;
            }
        }


        private async Task<Guid> AcceptPO(Guid order_id)
        {
            var command = new RecordAcceptedCommand { PurchaseOrderId = order_id, AcceptanceDate = DateTime.UtcNow, Accepted = true };
            return await _mediator.Send(command);
        }

        private async Task<Guid> RejectPO(Guid order_id)
        {
            var command = new RecordRejectedCommand { PurchaseOrderId = order_id, RejectionDate = DateTime.UtcNow, Rejected = true, Notes = "Rejected" };
            return await _mediator.Send(command);
        }

        private async Task<Guid> SetInProduction(Guid order_id)
        {
            var command = new RecordInProductionCommand { PurchaseOrderId = order_id, ProductionDate = DateTime.UtcNow, InProduction = true };
            return await _mediator.Send(command);
        }
        private async Task<Guid> SetPreShipped(Guid order_id)
        {
            var command = new RecordPreShipmentCommand { PurchaseOrderId = order_id, PreShipmentDate = DateTime.UtcNow, PreShipment = true };
            return await _mediator.Send(command);
        }

        private async Task<Guid> SetShipped(Guid order_id)
        {
            var command = new RecordShippingCommand { PurchaseOrderId = order_id, ShippedDate = DateTime.UtcNow, Shipping = true };
            return await _mediator.Send(command);
        }

        private async Task<Guid> SetComplete(Guid order_id)
        {
            var command = new RecordCompleteCommand { PurchaseOrderId = order_id, CompletionDate = DateTime.UtcNow, Complete = true };
            return await _mediator.Send(command);
        }


        private async Task PopulatePO()
        {
            Guid id = Guid.Parse(this.pid ?? "97690146-4b59-4ceb-a5fc-de7b7ac321fe");
            this.PO = await this.GetPurchaseOrder(id);

            this.ActionsAvailable = this.GetPOActions(this.PO.AvailableActions).OrderBy(o=>o.Value).ToList();

            this.ShowActions = false;
            if (this.ActionsAvailable.Count > 0)
            {
                this.ShowActions = this.ActionsAvailable.Count > 0;
                this.SelectedAction = this.ActionsAvailable.First().Value;
            }
        }


        private IEnumerable<SelectListItem> GetPOActions(PurchaseOrderActions avaiable_actions)
        {
            Func<string, string, SelectListItem> option = (value, text) => new SelectListItem
            {
                Value = value,
                Text = text
            };

            if (avaiable_actions.Accept || avaiable_actions.Reject) {
                yield return option("1-Accept", "Accept");
                yield return option("2-Reject", "Reject");
                yield break;
            }   

            if (avaiable_actions.SetProductionStatus)
            {
                yield return option("3-InProd", "In Production");
                yield return option("4-PreShip", "Pre-Shipping");
                yield return option("5-Shipping", "Shipping");
                yield return option("6-Complete", "Complete");
                yield break;
            }
            if (avaiable_actions.SetPreShippingStatus)
            {
                yield return option("4-PreShip", "Pre-Shipping");
                yield return option("5-Shipping", "Shipping");
                yield return option("6-Complete", "Complete");
                yield break;
            }
            if (avaiable_actions.SetShippingStatus)
            {
                yield return option("5-Shipping", "Shipping");
                yield return option("6-Complete", "Complete");
                yield break;
            }
            if (avaiable_actions.Complete)
            {
                yield return option("6-Complete", "Complete");
                yield break;
            }
            
            yield break;
        }


        private async Task<PurchaseOrderResource> GetPurchaseOrder(Guid id)
        {
            var p = await _context.PurchaseOrders.Where(q => q.Id == id).Select(s => new { PurchaseOrderId = s.Id, OrganisationId = s.CompanyId }).FirstOrDefaultAsync();
            this.ActivePurchaseOrderId = id.ToString();

            return await _mediator.Send(new GetPurchaseOrderByIdQuery
            {
                OrganisationId = p.OrganisationId,
                OrganisationType = Core.Enums.OrganisationTypeEnum.SHIPPER,
                PurchaseOrderId = id,
                includeItems = true
            });
        }

    }
}
