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

namespace Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest
{
    public class GetQuotationRequestResourceQuery : IRequest<QuotationRequestResource>
    {
        public Guid QuotationRequestId { get; set; }
        public Guid OrganisationId { get; set; }
        public OrganisationTypeEnum OrganisationType { get; set; }

        public class GetQuotationRequestResourceQueryHandler : IRequestHandler<GetQuotationRequestResourceQuery, QuotationRequestResource>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;
            private readonly ILogger<GetQuotationRequestResourceQueryHandler> _logger;

            // Using DI to inject infrastructure persistence Repositories
            public GetQuotationRequestResourceQueryHandler(IMediator mediator, TradefactDbContext context, ILogger<GetQuotationRequestResourceQueryHandler> logger)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<QuotationRequestResource> Handle(GetQuotationRequestResourceQuery request, System.Threading.CancellationToken cancellationToken)
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

                QuotationRequestResource result = quotation_request.Adapt<QuotationRequestResource>();
                result.FreightMovement = await _mediator.Send(new GetFreightMovementByIdQuery
                {
                    FreightMovementId = quotation_request.FreightMovementId,
                    OrganisationId = request.OrganisationId
                });

                if (quotation_request.State != QuotationStateEnum.PENDING )
                {
                    var quote = await _context.Quotations
                        .Include(i => i.OriginCharges)
                        .Include(i => i.FreightCharges)
                        .Include(i => i.DestinationCharges)
                        .Include(i => i.AdditionalCharges)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(q => q.QuotationRequestId == quotation_request.Id && q.IsActive);

                    if (quote != null)
                    {
                        result.Quotation = quote.Adapt<QuotationResource>();
                        result.Revision = quote.Revision;
                        if (quote.LoadType.HasValue) { 
                            result.FreightMovement.LoadType = (int)quote.LoadType; 
                        }
                    }
                }
                return result;
            }
        }
    }
}
