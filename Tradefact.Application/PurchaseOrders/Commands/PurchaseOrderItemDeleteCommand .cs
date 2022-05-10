using Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderItemDeleteCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public Guid PurchaseOrderItemId { get; private set; }

        public PurchaseOrderItemDeleteCommand(Guid organisationId, Guid purchaseOrderId, Guid purchaseOrderItemId)
        {
            OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
            this.PurchaseOrderItemId = purchaseOrderItemId;
        }
    }

    public class PurchaseOrderItemDeleteCommandHandler : IRequestHandler<PurchaseOrderItemDeleteCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderItemDeleteCommandHandler> _logger;

        public PurchaseOrderItemDeleteCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderItemDeleteCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderItemDeleteCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders
                .Include(p => p.PurchaseOrderItems)
                .Include(o => o.AttachedProductDocuments)
                .FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            PurchaseOrderItem item = await _context.PurchaseOrderItems.SingleAsync(q => q.Id == command.PurchaseOrderItemId && q.PurchaseOrderId == command.PurchaseOrderId);
            item.IsActive = false;

            // Remove any product documents that no longer have anything to attach to
            if (existingPurchaseOrder.PurchaseOrderItems.Count(c => c.ProductId == item.ProductId && c.IsActive) == 0)
            {
                var attachments = existingPurchaseOrder.AttachedProductDocuments
                    .Where(d => d.PurchaseOrderProductId == item.ProductId);
                _context.RemoveRange(attachments);
            }

            _ = await _context.SaveChangesAsync();

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(command.PurchaseOrderId));

            return item.Id;
        }
    }
}
