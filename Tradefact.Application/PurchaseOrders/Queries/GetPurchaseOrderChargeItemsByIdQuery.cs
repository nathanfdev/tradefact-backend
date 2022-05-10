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
    public class GetPurchaseOrderChargeItemByIdQuery : IRequest<PurchaseOrderAdditionalChargeItemResource>
    {
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid PurchaseOrderChargeItemId { get; set; }


        public class GetPurchaseOrderChargeItemByIdQueryHandler : IRequestHandler<GetPurchaseOrderChargeItemByIdQuery, PurchaseOrderAdditionalChargeItemResource>
        {
            private readonly TradefactDbContext _context;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderChargeItemByIdQueryHandler(TradefactDbContext context, ICurrentUserService userService)
            {
                _context = context;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<PurchaseOrderAdditionalChargeItemResource> Handle(GetPurchaseOrderChargeItemByIdQuery request, System.Threading.CancellationToken cancellationToken)
            {
                PurchaseOrderChargeItem item = _context.PurchaseOrderChargeItems.FirstOrDefault(q => q.PurchaseOrderId == request.PurchaseOrderId && q.Id == request.PurchaseOrderChargeItemId);
                return item.Adapt<PurchaseOrderAdditionalChargeItemResource>();
            }
        }
    }
}
