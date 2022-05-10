using Core.Enums;
using Core.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Tradefact.Application.Shipments.Commands
{
    public class CreateShipmentCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public OrganisationTypeEnum OrganisationType { get; private set; }
        public Guid QuotationRequestId { get; private set; }
        public RouteSchedule Route { get; private set; }
        public CreateShipmentCommand()
        {

        }
        public CreateShipmentCommand(Guid organisationId, OrganisationTypeEnum organisationtype, Guid quotationRequestId, RouteSchedule route) : this()
        {
            OrganisationType = organisationtype;
            OrganisationId = organisationId;
            QuotationRequestId = quotationRequestId;
            Route = route;
        }
    }
}
