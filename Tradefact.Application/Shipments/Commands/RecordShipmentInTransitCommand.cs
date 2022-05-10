using Core.Models;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class RecordShipmentInTransitCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime DepartureDatePOL { get; set; }

        public class RecordShipmentInTransitCommandHandler : IRequestHandler<RecordShipmentInTransitCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordShipmentInTransitCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordShipmentInTransitCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                ShipmentStateMachine sm = new ShipmentStateMachine(entity.Status, entity.Stage);
                entity.InTransit = true;
                entity.InTransitDate = request.DepartureDatePOL;

               
                entity.Status = sm.CurrentStatus;
                entity.Stage = sm.CurrentStage;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }

    public class RecordShipmentDepartedPOLCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime DepartedDatePOL { get; set; }

        public class RecordShipmentDepartedPOLCommandHandler : IRequestHandler<RecordShipmentDepartedPOLCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordShipmentDepartedPOLCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordShipmentDepartedPOLCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                ShipmentStateMachine sm = new ShipmentStateMachine(entity.Status, entity.Stage);
                entity.InTransit = true;
                entity.DepartedPOL = true;
                entity.DepartedPOLDate = request.DepartedDatePOL;

                entity.Status = sm.CurrentStatus;
                entity.Stage = sm.CurrentStage;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }

    public class RecordShipmentArrivedPODCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime ArrivalDatePOD { get; set; }

        public class RecordShipmentArrivedPODCommandHandler : IRequestHandler<RecordShipmentArrivedPODCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordShipmentArrivedPODCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordShipmentArrivedPODCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                ShipmentStateMachine sm = new ShipmentStateMachine(entity.Status, entity.Stage);
                entity.InTransit = true;
                entity.ArrivedPOD = true;
                entity.ArrivedPODDate = request.ArrivalDatePOD;

                entity.Status = sm.CurrentStatus;
                entity.Stage = sm.CurrentStage;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }

    public class RecordIssueAtCustomsCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public DateTime IssueAtCustomsDate { get; set; }

        public class RecordIssueAtCustomsCommandHandler : IRequestHandler<RecordIssueAtCustomsCommand, Guid>
        {
            private readonly TradefactDbContext _context;

            public RecordIssueAtCustomsCommandHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(RecordIssueAtCustomsCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                ShipmentStateMachine sm = new ShipmentStateMachine(entity.Status, entity.Stage);
                entity.IssueAtCustoms = true;
                entity.IssueAtCustomsDate = request.IssueAtCustomsDate;

                entity.Status = sm.CurrentStatus;
                entity.Stage = sm.CurrentStage;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }


}
