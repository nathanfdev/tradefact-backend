using Dapper;
using Mapster;
using MediatR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Data;
using Core.Models;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderChargeItemsQuery : IRequest<List<PurchaseOrderAdditionalChargeItemResource>>
    {
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }

        public class GetPurchaseOrderChargeItemsQueryHandler : IRequestHandler<GetPurchaseOrderChargeItemsQuery, List<PurchaseOrderAdditionalChargeItemResource>>
        {
            private readonly TradefactDbContext _context;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderChargeItemsQueryHandler(TradefactDbContext context, ICurrentUserService userService)
            {
                _context = context;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<List<PurchaseOrderAdditionalChargeItemResource>> Handle(GetPurchaseOrderChargeItemsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                List<PurchaseOrderChargeItem> items = _context.PurchaseOrderChargeItems.Where(q => q.PurchaseOrderId == request.PurchaseOrderId && q.IsActive).OrderBy(o=>o.CreationDateInternal).ToList();
                return items.Adapt<List<PurchaseOrderAdditionalChargeItemResource>>();
            }
        }
    }
}
