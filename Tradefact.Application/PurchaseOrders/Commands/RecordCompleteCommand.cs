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
    public class RecordCompleteCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }

        public bool Complete { get; set; }
        public DateTime CompletionDate { get; set; }

        public class RecordCompleteCommandHandler : IRequestHandler<RecordCompleteCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public RecordCompleteCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(RecordCompleteCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                entity.Status = PurchaseOrderStatus.Complete;
                entity.Completed = true;
                entity.CompletedDate = request.CompletionDate;
                entity.IsLocked = true;

                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.Complete, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
