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
    public class RecordPreShipmentCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }

        public bool PreShipment { get; set; }
        public DateTime PreShipmentDate { get; set; }

        public class RecordPreShipmentCommandHandler : IRequestHandler<RecordPreShipmentCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public RecordPreShipmentCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(RecordPreShipmentCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                entity.Status = PurchaseOrderStatus.PreShipment;
                entity.PreShipment = true;
                entity.PreShipmentDate = request.PreShipmentDate;

                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.PreShipment, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
