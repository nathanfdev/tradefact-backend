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
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderChargeItemPatchCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public Guid PurchaseOrderChargeItemId { get; private set; }

        public JsonPatchDocument<UpdatePurchaseOrderAdditionalChargeRequest> PatchDoc { get; private set; }

        public PurchaseOrderChargeItemPatchCommand()
        {

        }

        public PurchaseOrderChargeItemPatchCommand(Guid organisationId, Guid purchaseOrderId, Guid itemId, JsonPatchDocument<UpdatePurchaseOrderAdditionalChargeRequest> patchDoc)
        {
            this.OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
            this.PurchaseOrderChargeItemId = itemId;
            this.PatchDoc = patchDoc;
        }
    }

    public class PurchaseOrderChargeItemPatchCommandHandler : IRequestHandler<PurchaseOrderChargeItemPatchCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderChargeItemPatchCommandHandler> _logger;

        public PurchaseOrderChargeItemPatchCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderChargeItemPatchCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderChargeItemPatchCommand command, CancellationToken cancellationToken)
        {
            if (command.PatchDoc == null)
            {
                throw new BadRequestException("Bad Request");
            }

            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId && c.CompanyId == command.OrganisationId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            // Get Purchase Order Item
            var existingPurchaseOrderChargeItem = await _context.PurchaseOrderChargeItems.FirstOrDefaultAsync(c => c.PurchaseOrderId == command.PurchaseOrderId && c.Id == command.PurchaseOrderChargeItemId, cancellationToken);
            // Do patch operation
            UpdatePurchaseOrderAdditionalChargeRequest request = new UpdatePurchaseOrderAdditionalChargeRequest();
            request = existingPurchaseOrderChargeItem.Adapt(request);
            command.PatchDoc.ApplyTo(request);

            existingPurchaseOrderChargeItem = request.Adapt(existingPurchaseOrderChargeItem);
            _ = await _context.SaveChangesAsync(cancellationToken);

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(command.PurchaseOrderId));

            return existingPurchaseOrder.Id;
        }

    }
}
