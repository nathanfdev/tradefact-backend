using Core.Enums;
using Core.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Tradefact.Application.Shipments.Commands
{
    public class AmendShipmentScheduleCommand : IRequest<Guid>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid ShipmentId { get; set; }

        public Decimal? Charge { get; set; }
        public DateTimeOffset GoodsReady { get; set; }
        public DateTimeOffset? DeparturelDate { get; set; }
        public DateTimeOffset? ArrivalDate { get; set; }
        public RouteSchedule Route { get; set; }
        public bool IsManualRoute { get; set; }
        public bool IsReSchedule { get; set; } = false;

        public AmendShipmentScheduleCommand()
        {

        }
    }
}
