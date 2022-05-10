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
    public class PurchaseOrderDeleteCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }

        public PurchaseOrderDeleteCommand(Guid organisationId, Guid purchaseOrderId)
        {
            OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
        }
    }

    public class PurchaseOrderDeleteCommandHandler : IRequestHandler<PurchaseOrderDeleteCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderDeleteCommandHandler> _logger;

        public PurchaseOrderDeleteCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderDeleteCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderDeleteCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            existingPurchaseOrder.IsActive = false;
            existingPurchaseOrder.Deleted = true;
            existingPurchaseOrder.DeletionDate = DateTime.Now;
            _ = await _context.SaveChangesAsync();

            _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(existingPurchaseOrder.Id, PurchaseOrderEventType.PODelete, existingPurchaseOrder.DeletionDate));

            return existingPurchaseOrder.Id;
        }


    }
}
