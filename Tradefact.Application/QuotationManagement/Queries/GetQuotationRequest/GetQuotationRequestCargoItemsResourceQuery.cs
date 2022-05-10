using Core.Common;
using Core.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.FreightMovements;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest
{
    public class GetQuotationRequestCargoItemsResourceQuery : IRequest<IPagedList<CargoItemResource>>
    {
        public Guid QuotationRequestId { get; set; }
        public Guid? ScheduleId { get; set; }
        public Guid OrganisationId { get; set; }
        public PagedResultParameters Paging { get; set; }
        public string ExistingFreightMovementItemId { get; set; }

        public OrganisationTypeEnum OrganisationType { get; set; }

        public class GetQuotationRequestCargoItemsResourceQueryHandler : IRequestHandler<GetQuotationRequestCargoItemsResourceQuery, IPagedList<CargoItemResource>>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;
            private readonly ILogger<GetQuotationRequestCargoItemsResourceQueryHandler> _logger;

            // Using DI to inject infrastructure persistence Repositories
            public GetQuotationRequestCargoItemsResourceQueryHandler(IMediator mediator, TradefactDbContext context, ILogger<GetQuotationRequestCargoItemsResourceQueryHandler> logger)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<IPagedList<CargoItemResource>> Handle(GetQuotationRequestCargoItemsResourceQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var query = _context.QuotationRequests
                    .Include(i => i.Partnership.Client)
                    .Include(i => i.Partnership.Provider)
                    .Where(q => q.Id == request.QuotationRequestId);

                if (request.OrganisationType == OrganisationTypeEnum.SHIPPER)
                    query = query.Where(q => q.Partnership.ClientId == request.OrganisationId);
                else
                    query = query.Where(q => q.Partnership.ProviderId == request.OrganisationId);

                var quotation_request = await query.AsNoTracking().SingleOrDefaultAsync();
                if (quotation_request == null)
                {
                    throw new NotFoundException(nameof(QuotationResource), request.QuotationRequestId);
                }
                IPagedList<CargoItemResource> result = await _mediator.Send(new GetFreightMovementCargoItemsByIdQuery
                {
                    FreightMovementId = quotation_request.FreightMovementId,
                    OrganisationId = request.OrganisationId,
                    Paging=request.Paging,
                    ExistingFreightMovementItemId=request.ExistingFreightMovementItemId,
                    ScheduleId = request.ScheduleId
                });
                return result;
            }
        }
    }
}
