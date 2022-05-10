using Core.Enums;
using MediatR;
using Tradefact.Data;
using Core.Interfaces;

namespace Tradefact.Api.Controllers
{
    public partial class NetworkController : DirectoryController
    {
        public override OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.SHIPPER;

        public NetworkController(TradefactDbContext context, IMediator mediator, ITradefactActivityService tradefactActivityService, ILocationService locationService) : base(context, mediator, tradefactActivityService, locationService) { }
    }
}