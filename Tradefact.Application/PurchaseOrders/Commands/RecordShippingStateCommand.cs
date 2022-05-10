using Core.Models;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class RecordShippingCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }

        public bool Shipping { get; set; }
        public DateTime ShippedDate { get; set; }

        public class RecordShippingCommandHandler : IRequestHandler<RecordShippingCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public RecordShippingCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(RecordShippingCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                entity.Status = PurchaseOrderStatus.Shipping;
                entity.Shipping = true;
                entity.ShippedDate = request.ShippedDate;
                
                //  When PO is marked as shipped we should always lock the order
                entity.IsLocked = true;


                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.Shipping, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
