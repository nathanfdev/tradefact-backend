using Core.Common;
using Core.Enums;
using Core.Models.Criteria;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Models;
using Tradefact.Application.Shipments.Queries.GetShipmentList;
using X.PagedList;

namespace Tradefact.Application.Shipments.Queries.GetExceptionsList
{
    public class GetExceptionsListQuery : IRequest<IPagedList<ExceptionsInfo>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public ShipmentSearchCriteria Criteria { get; set; }

        public PagedResultParameters Paging { get; set; }

        public class GetExceptionsListQueryHandler : IRequestHandler<GetExceptionsListQuery, IPagedList<ExceptionsInfo>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetExceptionsListQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<IPagedList<ExceptionsInfo>> Handle(GetExceptionsListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var sQuery = new ShipmentQueries().GetBuildQuery(request.Criteria, request.OrganisationType, _userService.Email);

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        request.OrganisationId,
                        @ShipmentMethod = request.Criteria.ShipmentMethod.GetValueOrDefault(),
                        @search = BuildSearchParam(request.Criteria.Search),
                        @PageSize = int.MaxValue,
                        @PageNumber = 1
                    }, commandTimeout: 60))
                    {
                        List<ExceptionsInfo> query_result_shipments = (await multi.ReadAsync<ExceptionsInfo>()).ToList();

                        IPagedList<ExceptionsInfo> shipments_with_exceptions = await query_result_shipments.Where(q => q.HasExceptions).ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                        return shipments_with_exceptions;
                    }
                }
            }

            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }
        }
    }
}
