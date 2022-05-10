using Core.Dtos.PurchaseOrder;
using Core.Models;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderItemDeleteAllCommand  : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }

        public PurchaseOrderItemDeleteAllCommand (Guid organisationId, Guid purchaseOrderId)
        {
            OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
        }
    }

    public class PurchaseOrderItemDeleteAllCommandHandler : IRequestHandler<PurchaseOrderItemDeleteAllCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderItemDeleteAllCommandHandler> _logger;
        private readonly IDbConnection _connection;

        public PurchaseOrderItemDeleteAllCommandHandler(IDbConnection connection, IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderItemDeleteAllCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = connection;
        }

        public async Task<Guid> Handle(PurchaseOrderItemDeleteAllCommand  command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            using (IDbConnection conn = _connection)
            {
                conn.Open();

                var delete_result = conn.Execute("DELETE FROM [dbo].PurchaseOrderItems WHERE PurchaseOrderId = @PurchaseOrderId", new { @PurchaseOrderId = command.PurchaseOrderId });

                conn.Close();
            }

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(command.PurchaseOrderId));

            return existingPurchaseOrder.Id;
        }


    }
}
