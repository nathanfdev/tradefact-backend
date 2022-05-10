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
    public class PurchaseOrderChargeItemCreateCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public CreatePurchaseOrderAdditionalChargeRequest Upsert { get; private set; }

        public PurchaseOrderChargeItemCreateCommand()
        {

        }

        public PurchaseOrderChargeItemCreateCommand(Guid organisationId, Guid purchaseOrderId, CreatePurchaseOrderAdditionalChargeRequest upsert)
        {
            this.OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
            this.Upsert = upsert;
        }
    }

    public class PurchaseOrderChargeItemCreateCommandHandler : IRequestHandler<PurchaseOrderChargeItemCreateCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderChargeItemCreateCommandHandler> _logger;

        public PurchaseOrderChargeItemCreateCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderChargeItemCreateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderChargeItemCreateCommand cmd, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == cmd.PurchaseOrderId && c.CompanyId == cmd.OrganisationId, cancellationToken);

            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", cmd.PurchaseOrderId);

            PurchaseOrderChargeItem item = new PurchaseOrderChargeItem();
            item.Id = Guid.NewGuid();
            item.PurchaseOrderId = cmd.PurchaseOrderId;

            item.Description = cmd.Upsert.Description;
            item.Quantity = cmd.Upsert.Quantity;
            item.Rate = cmd.Upsert.Rate;
            item.Type = cmd.Upsert.Type;

            existingPurchaseOrder.AdditionalCharges.Add(item);
            _ = await _context.SaveChangesAsync();

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(cmd.PurchaseOrderId));

            return item.Id;
        }

    }
}
