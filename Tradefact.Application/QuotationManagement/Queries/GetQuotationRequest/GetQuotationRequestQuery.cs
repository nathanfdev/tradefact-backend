using Core.Common;
using Core.Enums;
using Core.Models;
using Core.Models.Criteria;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models;
using Tradefact.Data;
namespace Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest
{
    public class GetQuotationRequestQuery : IRequest<QuotationRequest>
    {
        public Guid QuotationRequestId { get; set; }
        public Guid OrganisationId { get; set; }
        public OrganisationTypeEnum OrganisationType { get; set; }

        public class GetQuotationRequestQueryHandler : IRequestHandler<GetQuotationRequestQuery, QuotationRequest>
        {
            private readonly TradefactDbContext _context;
            public GetQuotationRequestQueryHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<QuotationRequest> Handle(GetQuotationRequestQuery request, System.Threading.CancellationToken cancellationToken)
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

                var movement = await _context.FreightMovements
                    .Include(i => i.PortOfLoading)
                    .Include(i => i.PortOfDischarge)
                    .Include(i => i.PlaceOfLoading)
                    .Include(i => i.PlaceOfDispatch)
                    .Include(i => i.Items).ThenInclude(i => i.CargoItems).ThenInclude(t => t.Product)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(q => q.Id == quotation_request.FreightMovementId);

                var freightMovementItems = FreightMovementHelpers.ExtractFCLandLCL(movement);

                quotation_request.FreightMovement = movement;

                return quotation_request;
            }
        }
    }
}
