using Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class RecordClearedCustomsCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime ClearedCustomsDate { get; set; }

        public class RecordClearedCustomsCommandHandler : IRequestHandler<RecordClearedCustomsCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordClearedCustomsCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordClearedCustomsCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                ShipmentStateMachine sm = new ShipmentStateMachine(entity.Status, entity.Stage);
                entity.CustomsClearence = true;
                entity.CustomsClearenceDate = request.ClearedCustomsDate;

                entity.Status = sm.CurrentStatus;
                entity.Stage = sm.CurrentStage;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }
}
