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
    public class RecordCollectedCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public bool Collected { get; set; }
        public DateTime CollectionDate { get; set; }

        public class RecordCollectedCommandHandler : IRequestHandler<RecordCollectedCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordCollectedCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordCollectedCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                entity.Collected = true;
                entity.CollectionDate = request.CollectionDate;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }
}
