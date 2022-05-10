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

namespace Tradefact.Application.Shipments.Commands
{
    public class AssignShipmentCollectionDateCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime CollectionDate { get; set; }

        public class AssignShipmentCollectionDateCommandHandler : IRequestHandler<AssignShipmentCollectionDateCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public AssignShipmentCollectionDateCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(AssignShipmentCollectionDateCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                entity.CollectionDate = request.CollectionDate;
                entity.EstimatedCollectionDate = request.CollectionDate;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }


}
