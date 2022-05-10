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
    public class RecordDraftCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }


        public class RecordDraftCommandHandler : IRequestHandler<RecordDraftCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public RecordDraftCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(RecordDraftCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                if (entity.Status >= PurchaseOrderStatus.Pending || entity.Status <= PurchaseOrderStatus.PreShipment)
                {
                    entity.Status = PurchaseOrderStatus.Draft;
                    entity.Submitted = false;
                    entity.SubmittedDate = null;
                }

                if(entity.Status == PurchaseOrderStatus.Rejected)
                {
                    entity.Status = PurchaseOrderStatus.Draft;
                    entity.Rejected = false;
                    entity.RejectedDate = null;
                    entity.RejectedReason = null;
                    entity.Submitted = false;
                    entity.SubmittedDate = null;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.Draft, DateTime.Now));
                return entity.Id;
            }
        }
    }
}
