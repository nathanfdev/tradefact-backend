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
    public class RecordInProductionCommand : IRequest<Guid>
    {
        public Guid? PurchaseOrderId { get; set; }

        public bool InProduction { get; set; }
        public DateTime ProductionDate { get; set; }

        public class RecordInProductionCommandHandler : IRequestHandler<RecordInProductionCommand, Guid>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;

            public RecordInProductionCommandHandler(TradefactDbContext context, IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
            }

            public async Task<Guid> Handle(RecordInProductionCommand request, CancellationToken cancellationToken)
            {

                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                entity.Status = PurchaseOrderStatus.Production;
                entity.InProduction = true;
                entity.InProductionDate = request.ProductionDate;

                await _context.SaveChangesAsync(cancellationToken);

                _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.Production, DateTime.Now));

                return entity.Id;
            }
        }
    }
}
