using Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class RecordDeliveryCompleteCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public bool Delivered { get; set; }
        public DateTime DeliveryCompleteDate { get; set; }

        public class RecordDeliveryCompleteCommandHandler : IRequestHandler<RecordDeliveryCompleteCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordDeliveryCompleteCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordDeliveryCompleteCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                entity.Delivered = true;
                entity.DeliveryDate = request.DeliveryCompleteDate;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }
}
