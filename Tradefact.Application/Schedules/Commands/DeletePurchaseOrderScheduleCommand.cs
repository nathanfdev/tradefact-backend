using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.Schedules.Commands
{

    public class DeletePurchaseOrderScheduleCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid ScheduleLineId { get; set; }

        public DeletePurchaseOrderScheduleCommand(Guid organisationId, Guid scheduleLineId)
        {
            this.OrganisationId = organisationId;
            this.ScheduleLineId = scheduleLineId;
        }
    }

    public class DeletePurchaseOrderScheduleCommandHandler : IRequestHandler<DeletePurchaseOrderScheduleCommand, Guid>
    {
        private readonly TradefactDbContext _context;
        private readonly ILogger<DeletePurchaseOrderScheduleCommandHandler> _logger;

        public DeletePurchaseOrderScheduleCommandHandler(TradefactDbContext context, ILogger<DeletePurchaseOrderScheduleCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(DeletePurchaseOrderScheduleCommand command, CancellationToken cancellationToken)
        {
            var existingScheduleLine = await _context.PurchaseOrderItemScheduleLines.Include(i=>i.PurchaseOrderItem).ThenInclude(i=>i.PurchaseOrder).FirstOrDefaultAsync(c => c.Id == command.ScheduleLineId, cancellationToken);
            if (existingScheduleLine == null || existingScheduleLine.PurchaseOrderItem.PurchaseOrder.CompanyId != command.OrganisationId) throw new NotFoundException("ScheduleLine", command.ScheduleLineId);

            foreach (var schedule in _context.PurchaseOrderItemScheduleLines.Where(c => c.Id == command.ScheduleLineId).Include(i => i.PurchaseOrderItem).ThenInclude(i => i.PurchaseOrder)) {
                schedule.IsActive = false;
            }

            // If the last schedule has been deleted unlock the PO
            if (_context.PurchaseOrderItemScheduleLines.Where(q => 
                q.PurchaseOrderId == existingScheduleLine.PurchaseOrderId && 
                q.Id != existingScheduleLine.Id &&
                q.IsActive).Count() == 0)
            {
                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(q => q.Id == existingScheduleLine.PurchaseOrderId);
                if (po.Status < Core.Models.PurchaseOrderStatus.Shipping)
                {
                    po.IsLocked = false;
                }
            }

            _ = await _context.SaveChangesAsync();

            return command.ScheduleLineId;
        }


    }
}
