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
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Application.Shipments.Queries.GetShipmentInfo;
using Tradefact.Data;
using Tradefact.Portal.Infrastructure.Filters;

namespace Tradefact.Portal.Pages
{
    //[ValidateSharedURLSignature]
    public class PackingListModel : PageModel
    {

        [FromQuery(Name = "sid")]
        public string sid { get; set; }

        [FromQuery(Name = "sig")]
        public string sig { get; set; }

        [BindProperty]
        [HiddenInput]
        public string ActiveShipmentId { get; set; }

        [BindProperty]
        public string ID { get; set; }

        [BindProperty]
        public List<PackingListItem> AssignedSKUQuantities { get; set; }

        public bool ShowActions { get; set; } = false;

        private readonly TradefactDbContext _context;
        private readonly ILogger<POViewerModel> _logger;
        protected readonly IMediator _mediator;

        public ShipmentPackingListInfo Shipment { get; set; }

        public PackingListModel(TradefactDbContext context, IMediator mediator, ILogger<POViewerModel> logger)
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
            await this.PopulatePackingList();
        }

        public async Task<IActionResult> OnPost()
        {
            Guid purchase_order_id = await this.UpdatePOStatus(this.ActiveShipmentId);

            return new RedirectToPageResult("POAcknowledged")
            {
                RouteValues = new RouteValueDictionary
                    {
                        { "pid", sid.ToString() },
                        { "sig", sig.ToString() }
                    }
            };
        }

        private async Task<Guid> UpdatePOStatus(string order_id)
        {
            Guid id = Guid.Parse(this.ActiveShipmentId);
            return id;
        }


        private async Task<Guid> AcceptPO(Guid order_id)
        {
            var command = new RecordAcceptedCommand { PurchaseOrderId = order_id, AcceptanceDate = DateTime.UtcNow, Accepted = true };
            return await _mediator.Send(command);
        }

        private async Task PopulatePackingList()
        {
            Guid id = Guid.Parse(this.sid ?? "77b83692-9183-4301-8347-e9180153fb28");
            this.Shipment = await this.GetShipment(id);
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


        private async Task<ShipmentPackingListInfo> GetShipment(Guid id)
        {
            var shipment = await _context.Shipments
                .Include(i => i.Partnership).ThenInclude(t => t.Client)
                .Include(i => i.Partnership).ThenInclude(t => t.Provider)
                .Where(q => q.Id == id).Select(s => new ShipmentInformation
                { 
                    ShipmentId = s.Id, 
                    Shipper = s.Partnership.Client,
                    Forwarder = s.Partnership.Provider
                }).FirstOrDefaultAsync();
            this.ActiveShipmentId = id.ToString();

            Guid _supplierId = new Guid("3a68a292-44aa-4038-bf11-922aa7774fe9");
            Guid _placeOfLoadingId = new Guid("0a5f3be6-dd91-4afd-e355-08d863a4111c");

            ShipmentPackingListInfo pk = await _mediator.Send(new GetShipmentPackingListQuery { ShipmentId = id, ShipperId = shipment.Shipper.Id, SupplierId = _supplierId , PlaceOfLoadingId = _placeOfLoadingId });
            return pk;
        }

        public IActionResult OnPostAssignItemQuantities()
        {
            foreach (string key in Request.Form.Keys)
            {
                Console.WriteLine(key);
            }

            return Redirect("Index/");
        }

        public async Task<JsonResult> OnGetPackingListItems(Guid shipmentId)
        {
            ShipmentPackingListInfo packing_list = await this.GetShipment(shipmentId);

            string data = JsonConvert.SerializeObject(packing_list);

            var result = packing_list.Products.Select(s => new { id = s.PurchaseOrderItemId, desc = s.Product_Description, sku = s.Product_SKU, qty = s.QtyCommitted }).ToList();
            return new JsonResult(result);
        }


        private ShipmentPackingListInfo GetLocalShipment(Guid shipmentId)
        {

            // Read the file as one string.
            string text = System.IO.File.ReadAllText(@"C:\Users\patri\Downloads\shipment_data_packing_list.json");
            return JsonConvert.DeserializeObject<ShipmentPackingListInfo>(text);

        }

    }

    public class ShipmentInformation
    {
        public Guid ShipmentId { get; set; }
        public Organisation Shipper { get; set; }
        public Organisation Forwarder { get; set; }
        public ShipmentInfo Shipment { get; set; }
    }

    public class PackingListItem
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public string PalletNo { get; set; }
        public string CartonQty { get; set; }
        public string CartonWeight { get; set; }
        public string CartonLxWxHx { get; set; }
        public string Weight { get; set; }
    }
}
