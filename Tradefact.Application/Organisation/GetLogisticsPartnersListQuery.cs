using Core.Common;
using Core.Enums;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Application.Organisation
{
    public class GetLogisticsPartnersListQuery : IRequest<IPagedList<PartnershipResource>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public PagedResultParameters Paging { get; set; }
        public string Search { get; set; }

        public class GetLogisticsPartnersListQueryHandler : IRequestHandler<GetLogisticsPartnersListQuery, IPagedList<PartnershipResource>>
        {
            private readonly TradefactDbContext _context;

            public GetLogisticsPartnersListQueryHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<IPagedList<PartnershipResource>> Handle(GetLogisticsPartnersListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                IQueryable<Partnership> query = _context.Partnerships.Include(i=>i.Client).Include(i => i.Provider).Where(q=>q.IsActive);

                if (request.OrganisationType == OrganisationTypeEnum.SHIPPER)
                {
                    query = query.Where(q => q.ClientId == request.OrganisationId);

                    if (request.Search != null && request.Search.Length > 1)
                    {
                        query = query.Where(q => EF.Functions.Like(q.Provider.Name, $"%{request.Search}%"));
                    }
                } else
                {
                    query = query.Where(q => q.ProviderId == request.OrganisationId);

                    if (request.Search != null && request.Search.Length > 1)
                    {
                        query = query.Where(q => EF.Functions.Like(q.Client.Name, $"%{request.Search}%"));
                    }
                }
                IPagedList<PartnershipResource> partnerships = await query.Adapt<List<PartnershipResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                return partnerships;
            }

        }
    }
}
