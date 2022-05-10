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
    public class PurchaseOrderPatchCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public JsonPatchDocument<PurchaseOrderEditDto> PatchDoc { get; private set; }
        public PurchaseOrderPatchCommand(Guid organisationId, Guid purchaseOrderId, JsonPatchDocument<PurchaseOrderEditDto> patchDoc)
        {
            this.PatchDoc = patchDoc;
            OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
        }
    }

    public class PurchaseOrderPatchCommandHandler : IRequestHandler<PurchaseOrderPatchCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderPatchCommandHandler> _logger;

        public PurchaseOrderPatchCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderPatchCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderPatchCommand patchPurchaseOrderCommand, CancellationToken cancellationToken)
        {
            if (patchPurchaseOrderCommand.PatchDoc == null)
            {
                throw new BadRequestException("Bad Request");
            }

            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == patchPurchaseOrderCommand.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", patchPurchaseOrderCommand.PurchaseOrderId);

            // Do patch operation
            PurchaseOrderEditDto request = new PurchaseOrderEditDto();
            request = existingPurchaseOrder.Adapt(request);
            patchPurchaseOrderCommand.PatchDoc.ApplyTo(request);

            existingPurchaseOrder = request.Adapt(existingPurchaseOrder);

            _ = await _context.SaveChangesAsync(cancellationToken);

            return existingPurchaseOrder.Id;
        }

    }
}
