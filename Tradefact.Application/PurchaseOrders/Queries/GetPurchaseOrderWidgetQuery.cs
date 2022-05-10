using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using X.PagedList;
using Mapster;
using Core.Models;
using System.Linq;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderWidgetQuery : IRequest<PurchaseOrderWidgetResource>
    {
        public Guid OrganisationId { get; set; }

        public class GetPurchaseOrderWidgetQueryHandler : IRequestHandler<GetPurchaseOrderWidgetQuery, PurchaseOrderWidgetResource>
        {
            private readonly TradefactDbContext _context;

            public GetPurchaseOrderWidgetQueryHandler(TradefactDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<PurchaseOrderWidgetResource> Handle(GetPurchaseOrderWidgetQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var statusNotIncluded = new List<PurchaseOrderStatus>
                {
                    PurchaseOrderStatus.Draft,
                    PurchaseOrderStatus.Cancelled,
                    PurchaseOrderStatus.Complete,
                    PurchaseOrderStatus.Archived
                };

                var activeStatuses = new List<PurchaseOrderStatus>
                {
                    PurchaseOrderStatus.Pending,
                    PurchaseOrderStatus.Accepted,
                    PurchaseOrderStatus.Production,
                    PurchaseOrderStatus.PreShipment,
                    PurchaseOrderStatus.Shipping
                };

                var widgetResult = new PurchaseOrderWidgetResource();

                var test = from p in _context.PurchaseOrders
                    join c in _context.Organisations on p.SupplierId equals c.Id
                    where p.CompanyId == request.OrganisationId && p.AcceptedDate != null
                    orderby p.AcceptedDate descending
                    select new PurchaseOrderWidgetAcceptedOrders
                    {
                        Id = p.Id,
                        PoNumber = p.PurchaseOrderNumber,
                        CurrencyCode = p.CurrencyId,
                        AcceptedDate = p.AcceptedDate,
                        SupplierName = c.Name
                    };

                var recentlyAcceptedOrders = await test.Take(3).ToListAsync();

                widgetResult.AcceptedOrders = recentlyAcceptedOrders;
                widgetResult.StatusInfo = new PurchaseOrderWidgetStatusInfo
                {
                    Pending = new PurchaseOrderWidgetStatusCalculations
                    {
                        Total = 0,
                        Percentage = 0,
                        Color = "#FEA115"
                    },
                    Accepted = new PurchaseOrderWidgetStatusCalculations
                    {
                        Total = 0,
                        Percentage = 0,
                        Color = "#2BC585"
                    },
                    Shipping = new PurchaseOrderWidgetStatusCalculations
                    {
                        Total = 0,
                        Percentage = 0,
                        Color = "#2C7BE5"
                    },
                    Rejected = new PurchaseOrderWidgetStatusCalculations
                    {
                        Total = 0,
                        Percentage = 0,
                        Color = "#E66054"
                    }
                };

                var activeTotal = 0;

                var groupedStatuses = await _context.PurchaseOrders
                        .Where(q => q.CompanyId == request.OrganisationId && !statusNotIncluded.Contains(q.Status))
                        .GroupBy(info => info.Status)
                        .Select(group => new
                        {
                            Status = group.Key,
                            Count = group.Count()
                        }).ToListAsync();

                foreach (var item in groupedStatuses)
                {
                    if (activeStatuses.Contains(item.Status))
                    {
                        activeTotal += item.Count;
                    }

                    if (item.Status == PurchaseOrderStatus.Pending)
                    {
                        widgetResult.StatusInfo.Pending.Total = item.Count;
                    }

                    if (item.Status == PurchaseOrderStatus.Accepted)
                    {
                        widgetResult.StatusInfo.Accepted.Total = item.Count;
                    }

                    if (item.Status == PurchaseOrderStatus.Shipping)
                    {
                        widgetResult.StatusInfo.Shipping.Total = item.Count;
                    }

                    if (item.Status == PurchaseOrderStatus.Rejected)
                    {
                        widgetResult.StatusInfo.Rejected.Total = item.Count;
                    }
                }

                widgetResult.ActiveTotal = activeTotal;

                var pendingTotal = widgetResult.StatusInfo.Pending.Total;
                var acceptedTotal = widgetResult.StatusInfo.Accepted.Total;
                var shippingTotal = widgetResult.StatusInfo.Shipping.Total;
                var rejectedTotal = widgetResult.StatusInfo.Rejected.Total;

                var overall = pendingTotal + acceptedTotal + shippingTotal + rejectedTotal;
                widgetResult.StatusInfo.Pending.Percentage = this.GetStatusPercentage(pendingTotal, overall);
                widgetResult.StatusInfo.Accepted.Percentage = this.GetStatusPercentage(acceptedTotal, overall);
                widgetResult.StatusInfo.Shipping.Percentage = this.GetStatusPercentage(shippingTotal, overall);
                widgetResult.StatusInfo.Rejected.Percentage = this.GetStatusPercentage(rejectedTotal, overall);

                return widgetResult;
            }

            private double GetStatusPercentage(int statusTotal, int overall)
            {
                return Math.Round(statusTotal * 100.0 / overall, 1);
            }
        }
    }
}
