using Core.Interfaces;
using Core.Models;
using MediatR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class AssignTransitInformationCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }

        public string Vessel { get; set; }
        public string VesselIMO { get; set; }
        public string BOLNumber { get; set; }
        public string Carrier { get; set; }
        public bool IsBOLNumberOnly { get; set; } = false;

        public List<string> ContainerIds { get; set; }

        public class AssignTransitInformationCommandHandler : IRequestHandler<AssignTransitInformationCommand, Guid>
        {
            private readonly TradefactDbContext _context;
            private readonly ISeaTrackingService _seaTrackingService;

            public AssignTransitInformationCommandHandler(TradefactDbContext context, ISeaTrackingService seaTrackingService = null)
            {
                _context = context;
                _seaTrackingService = seaTrackingService;
            }

            public async Task<Guid> Handle(AssignTransitInformationCommand request, CancellationToken cancellationToken)
            {

                Shipment entity = await _context.Shipments.FindAsync(request.Id.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.Id);
                }

                ShipmentStateMachine sm = new ShipmentStateMachine(entity.Status, entity.Stage);

                entity.TrackinformationAdded = true;
                entity.BillofLadingNumber = request.BOLNumber;
                entity.SCAC = request.Carrier;


                Carrier c = _context.Carriers.FirstOrDefault(q=>q.SCAC == entity.SCAC);
                if (c != null)
                {
                    request.ContainerIds = new List<string> { entity.BillofLadingNumber };
                }
                if (!request.IsBOLNumberOnly)
                {
                    entity.IMO = request.VesselIMO;
                    entity.VesselName = request.Vessel;
                    if (request.ContainerIds != null && request.ContainerIds.Count > 0)
                    {
                        entity.Equipment = new List<EquipmentItem>();
                        foreach (var item in request.ContainerIds)
                        {
                            entity.Equipment.Add(new EquipmentItem { Id = Guid.NewGuid(), ContainerNo = item, IsActive = true });
                        }
                        if (_seaTrackingService != null)
                        {
                            _ = await _seaTrackingService.InitiateContainerTrack(request.ContainerIds, c.Tracking.OperatorValue);
                        }
                    }
                }
                entity.InTransit = true;

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }

}
