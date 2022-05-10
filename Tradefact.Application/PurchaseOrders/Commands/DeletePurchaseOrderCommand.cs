using Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class DeletePurchaseOrderCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }
        public bool Delete { get; set; }
        public DateTime DeleteDate { get; set; }
        public string Notes { get; set; }

        public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public DeletePurchaseOrderCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
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


                entity.Status = PurchaseOrderStatus.Cancelled;
                entity.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.PODelete, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
