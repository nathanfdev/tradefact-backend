using Core.Caching;
using Core.Common.Caching;
using Core.Interfaces;
using Core.Models;
using Core.Models.Tracking;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Data;

namespace Tradefact.Application.Tracking
{
    public class GetBillTrackingQuery : IRequest<List<TrackingResource>>
    {
        public Guid ShipmentId { get; set; }


        public class GetBillTrackingQueryHandler : IRequestHandler<GetBillTrackingQuery, List<TrackingResource>>
        {
            private readonly TradefactDbContext _context;

            public GetBillTrackingQueryHandler(TradefactDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<TrackingResource>> Handle(GetBillTrackingQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var shipment = await _context.Shipments.Where(q => q.Id == request.ShipmentId).Include(i=>i.TrackingEvents).FirstOrDefaultAsync();
                
                if (shipment != null && shipment.TrackingEvents.Any())
                {
                    var lastevent = shipment.TrackingEvents.OrderByDescending(o => o.TimeOfEvent).FirstOrDefault();

                    var result = shipment.TrackingEvents.GroupBy(k => new { k.TrackingNumber, k.EquipmentItemId })
                        .Select(gp => new TrackingResource { BillNumber = gp.Key.EquipmentItemId, ContainerNo = gp.Key.EquipmentItemId, LastEvent = lastevent.Information, LastEventTime = lastevent.TimeOfEvent.ToString(),
                            TrackingEventResource = gp.Select(s => new TrackingEventResource { 
                                TimeOfEvent = s.TimeOfEvent,
                                Activity = s.Activity,
                                Information = s.Information,
                                LocationOfEvent = s.LocationOfEvent,
                                Voyage = s.Voyage
                        }).OrderBy(o=>o.TimeOfEvent).ToList() }).ToList();

                    return result;
                };
                return new List<TrackingResource>();
            }
        }
    }

}
