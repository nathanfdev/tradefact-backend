using Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class AssignShipmentEstimatedDeliveryDateCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public class AssignShipmentEstimatedDeliveryDateCommandHandler : IRequestHandler<AssignShipmentEstimatedDeliveryDateCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public AssignShipmentEstimatedDeliveryDateCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(AssignShipmentEstimatedDeliveryDateCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                entity.EstimatedDeliveryDate = request.EstimatedDeliveryDate;
                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }


}
