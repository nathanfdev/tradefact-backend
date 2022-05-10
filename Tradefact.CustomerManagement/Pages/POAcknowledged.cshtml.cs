using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Portal.Pages
{
    public class POAcknowledgedModel : PageModel
    {
        [FromQuery(Name = "pid")]
        public string pid { get; set; }

        [FromQuery(Name = "sig")]
        public string sig { get; set; }

        public PurchaseOrderResource PO { get; set; }

        [BindProperty]
        [HiddenInput]
        public string ActivePurchaseOrderId { get; set; }

        [BindProperty]
        [HiddenInput]
        public string ActiveSignature { get; set; }

        private readonly TradefactDbContext _context;
        private readonly ILogger<POAcknowledgedModel> _logger;
        protected readonly IMediator _mediator;

        public POAcknowledgedModel(TradefactDbContext context, IMediator mediator, ILogger<POAcknowledgedModel> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnGet()
        {
            Guid id = Guid.Parse(this.pid ?? "97690146-4b59-4ceb-a5fc-de7b7ac321fe");
            this.PO = await this.GetPurchaseOrder(id);

            this.ActivePurchaseOrderId = id.ToString();
            this.ActiveSignature = sig.ToString();
        }

        public IActionResult OnPostBack()
        {
            return new RedirectToPageResult("POViewer")
            {
                RouteValues = new RouteValueDictionary
                    {
                        { "pid", this.ActivePurchaseOrderId.ToString() },
                        { "sig", this.ActiveSignature.ToString() }
                    }
            };
        }

        private async Task<PurchaseOrderResource> GetPurchaseOrder(Guid id)
        {
            var p = await _context.PurchaseOrders.Where(q => q.Id == id).Select(s => new { PurchaseOrderId = s.Id, OrganisationId = s.CompanyId }).FirstOrDefaultAsync();

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
