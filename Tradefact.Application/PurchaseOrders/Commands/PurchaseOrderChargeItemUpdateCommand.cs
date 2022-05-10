using Core.Dtos.PurchaseOrder;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderChargeItemUpdateCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public Guid PurchaseOrderChargeItemId { get; private set; }
        public UpdatePurchaseOrderAdditionalChargeRequest Upsert { get; private set; }

        public PurchaseOrderChargeItemUpdateCommand()
        {

        }

        public PurchaseOrderChargeItemUpdateCommand(Guid organisationId, Guid purchaseOrderId, Guid purchaseOrderChargeItemId, UpdatePurchaseOrderAdditionalChargeRequest upsert)
        {
            this.OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
            this.PurchaseOrderChargeItemId = purchaseOrderChargeItemId;
            this.Upsert = upsert;
        }
    }

    public class PurchaseOrderChargeItemUpdateCommandHandler : IRequestHandler<PurchaseOrderChargeItemUpdateCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderChargeItemUpdateCommandHandler> _logger;

        public PurchaseOrderChargeItemUpdateCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderChargeItemUpdateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderChargeItemUpdateCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            PurchaseOrderChargeItem item = await _context.PurchaseOrderChargeItems.SingleAsync(q => q.Id == command.PurchaseOrderChargeItemId && q.PurchaseOrderId == command.PurchaseOrderId);
            item = command.Upsert.Adapt(item);
            _ = await _context.SaveChangesAsync();

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(command.PurchaseOrderId));

            return item.Id;
        }

    }
}
