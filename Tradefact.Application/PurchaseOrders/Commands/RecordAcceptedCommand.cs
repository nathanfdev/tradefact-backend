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
    public class RecordAcceptedCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }

        public bool Accepted { get; set; }
        public DateTime AcceptanceDate { get; set; }

        public class RecordAcceptedCommandHandler : IRequestHandler<RecordAcceptedCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public RecordAcceptedCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(RecordAcceptedCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                entity.Status = PurchaseOrderStatus.Accepted;
                entity.Accepted = true;
                entity.AcceptedDate = request.AcceptanceDate;

                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.Accepted, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
