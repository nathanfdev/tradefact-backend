using Core.Enums;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    public partial class BuyerController : DirectoryController
    {
        public override OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.BUYER;
        public BuyerController(TradefactDbContext context) : base(context) { }
    }
}