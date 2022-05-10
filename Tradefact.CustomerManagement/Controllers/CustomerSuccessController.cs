using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Data;
using Tradefact.Portal.Models;

namespace Tradefact.Portal.Controllers
{
    public class CustomerSuccessController : Controller
    {
        private readonly TradefactDbContext _context;

        public CustomerSuccessController(TradefactDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateQbrReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ShowQbrReport()
        {
            var org = await _context.Organisations.AsNoTracking().FirstOrDefaultAsync(q => q.Id == Guid.Parse(Request.Form["organisationId"]));
            var fromDate = DateTime.Parse(Request.Form["fromDate"], new CultureInfo("en-GB", false));
            var toDate = DateTime.Parse(Request.Form["toDate"], new CultureInfo("en-GB", false));

            if (org.OrganisationTypeId == OrganisationTypeEnum.PARTNER)
            {
                return View(await getPartnerMetrics(org, fromDate, toDate));

            }
            else
            {
                return View(await getShipperMetrics(org, fromDate, toDate));
            }
        }

        private async Task<ShowQbrReportModel> getShipperMetrics(Organisation org, DateTime fromDate, DateTime toDate)
        {
            var orders = await _context.PurchaseOrders.AsNoTracking()
                .Include(p => p.PurchaseOrderItems)
                .Where(q =>
                    q.CompanyId == org.Id &&
                    q.DateOfIssue >= fromDate &&
                    q.DateOfIssue < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var bookings = await _context.Shipments.AsNoTracking()
                .Include(s => s.FreightMovement)
                .Where(q =>
                    q.FreightMovement.CompanyId == org.Id &&
                    q.BookedDate >= fromDate &&
                    q.BookedDate < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var quotes = await _context.Quotations.AsNoTracking()
                .Include(q => q.QuotationRequest)
                .Where(q =>
                    q.FreightMovement.CompanyId == org.Id &&
                    q.IssueDate >= fromDate &&
                    q.IssueDate < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var invitations = await _context.InvitationLog.AsNoTracking()
                .Where(q =>
                    q.InviteRequestedByOrganisationId == org.Id &&
                    q.CreationDateInternal >= fromDate &&
                    q.CreationDateInternal < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var itemsInOrders = orders.Where(o => o.PurchaseOrderItems.Count() > 1).SelectMany(o => o.PurchaseOrderItems).ToList();
            var shippedOrders = orders.Where(o => o.ShippedDate != null);
            var deliveredBookings = bookings.Where(b => b.DepartedPOLDate != null && b.DeliveryDate != null);
            var orderToDelivery =
                from booking in deliveredBookings
                join order in shippedOrders on booking.FreightMovement.PurchaseOrderId equals order.Id
                select new { orderIssued = order.DateOfIssue, shipmentDelivered = booking.DeliveryDate };

            return new ShowQbrReportModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                OrganisationType = org.OrganisationTypeId.ToString(),
                CompanyName = org.Name,
                ContactName = org.ContactName,
                ContactEmail = org.ContactEmail,
                ContactTelephone = org.ContactTelephone,
                NumActiveSuppliers = orders.GroupBy(o => o.SupplierId).Count().ToString(),
                NumActiveLogistics = bookings.GroupBy(b => b.PartnershipId).Count().ToString(),
                NumPurchaseOrders = orders.Count().ToString(),
                NumDifferentSKUsOrdered = itemsInOrders.GroupBy(o => o.SKU).Count().ToString(),
                NumBookings = bookings.Count().ToString(),
                AvgTimeToShipDays = shippedOrders.Count() == 0 ? "N/A" : ((int)shippedOrders
                    .Average(o => (o.ShippedDate - o.DateOfIssue).Value.Days)).ToString() + " days",
                AvgTransitTimeDays = deliveredBookings.Count() == 0 ? "N/A" : ((int)deliveredBookings
                    .Average(b => (b.DeliveryDate - b.DepartedPOLDate).Value.Days)).ToString() + " days",
                AvgPurchaseOrderToDeliveryDays = orderToDelivery.Count() == 0 ? "N/A" : ((int)orderToDelivery
                    .Average(d => (d.shipmentDelivered - d.orderIssued).Value.Days)).ToString() + " days",
                AvgTimeToQuoteDays = quotes.Count() == 0 ? "N/A" : ((int)quotes
                    .Average(q => (q.IssueDate.DateTime - q.QuotationRequest.CreationDate.DateTime).Days)).ToString() + " days",
                NumInvitationsSent = invitations.Count().ToString(),
                NumInvitationsAccepted = invitations.Where(i => i.ActivationStatus > 0).Count().ToString()
            };
        }

        private async Task<ShowQbrReportModel> getPartnerMetrics(Organisation org, DateTime fromDate, DateTime toDate)
        {
            var bookings = await _context.Shipments.AsNoTracking()
                .Include(s => s.FreightMovement)
                .Where(q =>
                    q.Partnership.ProviderId == org.Id &&
                    q.BookedDate >= fromDate &&
                    q.BookedDate < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var quotes = await _context.Quotations.AsNoTracking()
                .Include(q => q.QuotationRequest)
                .Where(q =>
                    q.QuotationRequest.Partnership.ProviderId == org.Id &&
                    q.IssueDate >= fromDate &&
                    q.IssueDate < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var invitations = await _context.InvitationLog.AsNoTracking()
                .Where(q =>
                    q.InviteRequestedByOrganisationId == org.Id &&
                    q.CreationDateInternal >= fromDate &&
                    q.CreationDateInternal < toDate.AddDays(1) &&
                    q.IsActive).ToListAsync();

            var deliveredBookings = bookings.Where(b => b.DepartedPOLDate != null && b.DeliveryDate != null);

            return new ShowQbrReportModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                OrganisationType = org.OrganisationTypeId.ToString(),
                CompanyName = org.Name,
                ContactName = org.ContactName,
                ContactEmail = org.ContactEmail,
                ContactTelephone = org.ContactTelephone,
                NumBookings = bookings.Count().ToString(),
                AvgTransitTimeDays = deliveredBookings.Count() == 0 ? "N/A" : ((int)deliveredBookings
                    .Average(b => (b.DeliveryDate - b.DepartedPOLDate).Value.Days)).ToString() + " days",
                AvgTimeToQuoteDays = quotes.Count() == 0 ? "N/A" : ((int)quotes
                    .Average(q => (q.IssueDate.DateTime - q.QuotationRequest.CreationDate.DateTime).Days)).ToString() + " days",
                QuoteConversionRate = quotes.Count == 0 ? "N/A" : (bookings.Count() / quotes.Count() * 100).ToString() + "%",
                AvgFreightForwarderProfitMargin = quotes.Count() == 0 ? "N/A" : ((int)quotes.Average(q => q.Margin)).ToString() + "%",
                NumInvitationsSent = invitations.Count().ToString(),
                NumInvitationsAccepted = invitations.Where(i => i.ActivationStatus > 0).Count().ToString()
            };
        }


        [HttpGet]
        public async Task<string> OrganisationSearch(string search)
        {
            var result = await _context.Organisations.AsNoTracking()
                .Where(q => q.Name.ToLower().Contains(search.ToLower()))
                .OrderBy(q => q.Name)
                .Select(q => new OrganisationSearchModel
                {
                    OrganisationId = q.Id.ToString(),
                    OrganisationName = $"{q.Name} ({q.ContactEmail})"
                })
                .ToListAsync();

            return JsonConvert.SerializeObject(result);
        }
    }
}
