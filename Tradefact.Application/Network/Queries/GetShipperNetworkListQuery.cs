using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Models.Network;

namespace Tradefact.Application.Network.Queries
{
    public class GetShipperNetworkListQuery : IRequest<NetworkResource>
    {
        public Guid? OrganisationId { get; set; }

        public class GetShipperNetworkListQueryHandler : IRequestHandler<GetShipperNetworkListQuery, NetworkResource>
        {
            private readonly IDbConnection _connection;

            public GetShipperNetworkListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<NetworkResource> Handle(GetShipperNetworkListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string sQuery = $@"
                    DECLARE @Network TABLE
                    (   
                        CreationDateInternal DATETIME2 DEFAULT NULL,
                        Name NVARCHAR(60) DEFAULT NULL,
                        Status INT DEFAULT 0,
                        OrganisationType INT DEFAULT 0,
                        CountType INT Default 0
                    )

                    ;WITH cte_network AS (
                        SELECT O.CreationDateInternal, O.Name, B2B.ConnectionStatus [Status], 1 [OrganisationType], 0 [CountType]
                        FROM [dbo].[B2B_Connections] B2B
                            INNER JOIN [dbo].[Organisations] O ON O.Id = B2B.LinkedOrganisationId
                            LEFT JOIN [dbo].[B2B_ConnectionContacts] C ON C.ConnectionId = B2B.Id AND C.Active = 1
                        WHERE B2B.OrgansationId = @organisationId and B2B.Active = 1 AND B2B.ConnectionStatus = 1
                        GROUP BY B2B.LinkedOrganisationId, B2B.ConnectionStatus, O.Name, O.CreationDateInternal
                        UNION
                        SELECT AI.CreationDateInternal, AI.CompanyName [Name], 0 [Status], 1 [OrganisationType], 1 [CountType]
                            FROM [dbo].[AspNetUserInvitations] AI
                            LEFT JOIN [dbo].AspNetUsers AU ON AU.Email = AI.EmailAddress
                        WHERE AI.InviteRequestedByOrganisationId = @organisationId AND AI.ActivationStatus != 2 and AI.Active = 1 AND AI.InviteType = 2 and AU.Id IS NULL
                        UNION
                        SELECT P.CreationDateInternal, C.Name, 1 [Status], 4 [OrganisationType], 2 [CountType]
                        FROM [dbo].Partnerships P
                            LEFT JOIN [dbo].[Organisations] C ON C.Id = P.ProviderId
                        WHERE P.ClientId = @organisationId AND P.Active = 1 AND C.Active = 1  AND C.ContactEmail != 'logistics@tradefact.com'
                        GROUP BY P.Id, P.ProviderId, P.ClientId, P.CreationDateInternal, C.ContactEmail, C.Name
                        UNION
                        SELECT AI.CreationDateInternal, AI.CompanyName [Name], 0 [Status], 4 [OrganisationType], 3 [CountType]
                        FROM [dbo].[AspNetUserInvitations] AI
                            LEFT JOIN [dbo].[AspNetUsers] AU ON AU.Email = AI.EmailAddress
                        WHERE AI.InviteRequestedByOrganisationId = @organisationId AND AI.ActivationStatus != 2 and AI.Active = 1 AND AI.InviteType = 5 AND AU.Id IS NULL 
                    )
                    INSERT INTO @Network ( 
                        CreationDateInternal,
                        Name,
                        Status,
                        OrganisationType,
                        CountType
                    )
                    SELECT PS.CreationDateInternal, PS.Name, PS.Status, PS.OrganisationType, PS.CountType FROM cte_network PS
                    
                    SELECT CreationDateInternal, Name, Status, OrganisationType 
                        FROM @Network
                    ORDER BY CreationDateInternal desc

                    SELECT COUNT(*) FROM @Network N WHERE N.CountType = 0
                    SELECT COUNT(*) FROM @Network N WHERE N.CountType = 1
                    SELECT COUNT(*) FROM @Network N WHERE N.CountType = 2
                    SELECT COUNT(*) FROM @Network N WHERE N.CountType = 3
                ";


                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    using (var multi = await conn.QueryMultipleAsync(sQuery, new
                    {
                        @organisationId = request.OrganisationId,
                    }))
                    {
                        List<NetworkInfoResource> recentNetworks = (await multi.ReadAsync<NetworkInfoResource>()).Take(5).ToList();

                        int connectedCompanies = await multi.ReadFirstAsync<int>();
                        int pendingCompanies = await multi.ReadFirstAsync<int>();
                        int connectedLogistics = await multi.ReadFirstAsync<int>();
                        int pendingLogistics = await multi.ReadFirstAsync<int>();

                        var network_result = new NetworkResource
                        {
                            RecentNetworks = recentNetworks,
                            Company = new NetworkCountResource
                            {
                                PendingTotal = pendingCompanies,
                                ConnectedTotal = connectedCompanies
                            },
                            Logistics = new NetworkCountResource
                            {
                                PendingTotal = pendingLogistics,
                                ConnectedTotal = connectedLogistics
                            }
                        };

                        return network_result;
                    }
                }
            }
        }

    }
}
