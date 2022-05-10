using Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class ResetPurchaseOrderCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }

        public bool Reset { get; set; }
        public DateTime ResetDate { get; set; }
        public string Notes { get; set; }

        public class ResetPurchaseOrderCommandHandler : IRequestHandler<ResetPurchaseOrderCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public ResetPurchaseOrderCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(ResetPurchaseOrderCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                if (entity.IsLocked)
                {
                    throw new BadRequestException($"Purchase Order #{entity.PurchaseOrderNumber} is locked!");
                }


                entity.Status = PurchaseOrderStatus.Draft;
                entity.Accepted = false;
                entity.AcceptedDate = null;

                entity.InProduction = false;
                entity.InProductionDate = null;

                entity.PreShipment = false;
                entity.PreShipmentDate = null;

                entity.Rejected = false;
                entity.RejectedReason = null;
                entity.RejectedDate = null;

                entity.OrderRejected = false;

                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.Rejected, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
