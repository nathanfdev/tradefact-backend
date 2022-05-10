using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models.Shipment;
using Tradefact.Data;
using Microsoft.EntityFrameworkCore;

namespace Tradefact.Application.Shipments.Queries.GetShipmentInfo
{
    public class GetShipmentTransitInfoQuery : IRequest<ShipmentTransitInfo>
    {
        public Guid ShipmentId { get; set; }

        public class GetShipmentTransitInfoQueryHandler : IRequestHandler<GetShipmentTransitInfoQuery, ShipmentTransitInfo>
        {
            private readonly TradefactDbContext _context;

            public GetShipmentTransitInfoQueryHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<ShipmentTransitInfo> Handle(GetShipmentTransitInfoQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var query = _context.Shipments
                    .Where(q => q.Id == request.ShipmentId);

                ShipmentTransitInfo shipment = await _context.Shipments.Select(s => new ShipmentTransitInfo { Id = s.Id, SCAC = s.SCAC, IMO = s.IMO, VesselName = s.VesselName }).SingleOrDefaultAsync(q => q.Id == request.ShipmentId);

                return shipment;
            }


        }
    }
}
