using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Models;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetOrganisationalStatusQuery : IRequest<OrganisationStatusResource>
    {
        public Guid OrganisationId { get; set; }

        public class GetOrganisationalStatusQueryHandler : IRequestHandler<GetOrganisationalStatusQuery, OrganisationStatusResource>
        {
            private readonly IDbConnection _connection;
            private readonly ILogger<GetOrganisationalStatusQueryHandler> _logger;

            // Using DI to inject infrastructure persistence Repositories
            public GetOrganisationalStatusQueryHandler(IDbConnection connection, ILogger<GetOrganisationalStatusQueryHandler> logger)
            {
                _connection = connection ?? throw new ArgumentNullException(nameof(connection));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<OrganisationStatusResource> Handle(GetOrganisationalStatusQuery request, System.Threading.CancellationToken cancellationToken)
            {

                string sQuery = @$"SELECT Count(Active) [ActivePurchaseOrders] 
                                        FROM  [integration].[ExternalPurchaseOrders] EP
                                    WHERE EP.OrganisationId = @organisation_id
                                        AND EP.Imported = 0 AND EP.Active = 1;";

                int orders_count = 0;
                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    orders_count = await conn.ExecuteScalarAsync<int>(sQuery, new
                    {
                        @organisation_id = request.OrganisationId,
                    }, commandTimeout: 60);
                    return new OrganisationStatusResource
                    { 
                        ProfileComplete = false,
                        Inbox = new InboxStatusResource {
                            Orders = orders_count
                        }
                    };
                }
            }
        }
    }
}
