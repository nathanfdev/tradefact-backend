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
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderUpdateCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrderEditDto Upsert { get; private set; }

        public PurchaseOrderUpdateCommand(Guid organisationId, Guid purchaseOrderId, PurchaseOrderEditDto upsert)
        {
            Upsert = upsert;
            OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
        }
    }

    public class PurchaseOrderUpdateCommandHandler : IRequestHandler<PurchaseOrderUpdateCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderUpdateCommandHandler> _logger;

        public PurchaseOrderUpdateCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderUpdateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderUpdateCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            PurchaseOrder po = await _context.PurchaseOrders.SingleAsync(q => q.Id == command.PurchaseOrderId);
            po = command.Upsert.Adapt(po);
            _ = await _context.SaveChangesAsync();


            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(po.Id));

            return po.Id;
        }


    }
}
