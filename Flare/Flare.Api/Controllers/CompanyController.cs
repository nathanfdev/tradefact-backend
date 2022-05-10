using Core.Interfaces;
using Flare.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using System;

namespace Flare.Api.Controllers
{
    public partial class CompanyController : OrganisationController
    {
        private readonly ILogger<CompanyController> _logger;
        new private readonly IMediator _mediator;

        public CompanyController(FlareDbContext context, IMediator mediator, ILocationService locationService, ILogger<CompanyController> logger) : base(context, mediator, locationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
    }
}
