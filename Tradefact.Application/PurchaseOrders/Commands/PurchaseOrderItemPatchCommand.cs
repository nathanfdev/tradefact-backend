using Core.Dtos.PurchaseOrder;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
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
    public class PurchaseOrderItemPatchCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public Guid PurchaseOrderItemId { get; private set; }
        public JsonPatchDocument<PurchaseOrderItemEditDto> PatchDoc { get; private set; }
        public PurchaseOrderItemPatchCommand(Guid organisationId, Guid PurchaseOrderId, Guid PurchaseOrderItemId, JsonPatchDocument<PurchaseOrderItemEditDto> patchDoc)
        {
            this.PatchDoc = patchDoc;
            OrganisationId = organisationId;
            this.PurchaseOrderItemId = PurchaseOrderItemId;
        }
    }

    public class PurchaseOrderItemPatchCommandHandler : IRequestHandler<PurchaseOrderItemPatchCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderItemPatchCommandHandler> _logger;

        public PurchaseOrderItemPatchCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderItemPatchCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderItemPatchCommand cmd, CancellationToken cancellationToken)
        {
            if (cmd.PatchDoc == null)
            {
                throw new BadRequestException("Bad Request");
            }

            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == cmd.PurchaseOrderId && c.CompanyId == cmd.OrganisationId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", cmd.PurchaseOrderId);

            // Get Purchase Order Item
            var existingPurchaseOrderItem = await _context.PurchaseOrderItems.FirstOrDefaultAsync(c => c.Id == cmd.PurchaseOrderId && c.Id == cmd.PurchaseOrderItemId, cancellationToken);
            // Do patch operation
            PurchaseOrderItemEditDto request = new PurchaseOrderItemEditDto();
            request = existingPurchaseOrderItem.Adapt(request);
            cmd.PatchDoc.ApplyTo(request);

            existingPurchaseOrderItem = request.Adapt(existingPurchaseOrderItem);
            _ = await _context.SaveChangesAsync(cancellationToken);

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(cmd.PurchaseOrderId));

            return existingPurchaseOrder.Id;
        }

    }
}
