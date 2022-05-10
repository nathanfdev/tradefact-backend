using Core.Common;
using Core.Enums;
using Core.Models;
using Core.Models.Criteria;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Tradefact.Application.Products.Queries
{
    public class GetOrganisationsPerForwarderListQuery : IRequest<IPagedList<LogisticsClient>>
    {
        public Guid? OrganisationId { get; set; }
        public OrganisationSearchCriteria Criteria { get; set; }
        public string CompanyType { get; set; }
        public string RequestingType { get; set; }

        public int InviteType { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetOrganisationsPerForwarderListQueryHandler : IRequestHandler<GetOrganisationsPerForwarderListQuery, IPagedList<LogisticsClient>>
        {
            private readonly IDbConnection _connection;

            public GetOrganisationsPerForwarderListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<LogisticsClient>> Handle(GetOrganisationsPerForwarderListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder where_clause = new StringBuilder();
                StringBuilder pendingQueryBuilder = new StringBuilder();
                string where_confirmed_users = "";
                if (!String.IsNullOrEmpty(request.Criteria.Search) && request.Criteria.Search.Length > 0) where_clause.Append($" AND C.Name LIKE @search");

                if (request.Criteria.Status != null)
                {
                    if (request.Criteria.Status == ConfirmedUsersStatusEnum.PENDING)
                    {
                        where_confirmed_users = $"PS.ConfirmedUsers = 0";
                    }
                    else
                    {
                        where_confirmed_users = $"PS.ConfirmedUsers > 0";
                    }
                }
                else
                {
                    where_confirmed_users = $"PS.ConfirmedUsers >= 0";
                }

                if (request.Criteria.ShowPending)
                {
                    pendingQueryBuilder.AppendLine($@"
                        UNION
                        SELECT NULL [PartnershipId], AI.CreationDateInternal, NULL [ProviderId], NULL [ClientId], 0 [ActiveShipments], NULL [LastFreightMovementEntered], NULL [LastShipmentBooked], AI.EmailAddress [ContactEmail], AI.CompanyName [Name], 0 [ConfirmedUsers]
                        FROM [dbo].[AspNetUserInvitations] AI
                            LEFT JOIN [dbo].[AspNetUsers] AU ON AU.Email = AI.EmailAddress

                        WHERE AI.InviteRequestedByOrganisationId = @organisationId AND AI.ActivationStatus != 2 and AI.Active = 1 AND AI.InviteType = {request.InviteType.ToString()} AND AU.Id IS NULL 
                    ");
                }

                if (request.Criteria.CountryCode != null)
                {
                    where_confirmed_users += $" AND EXISTS (SELECT * FROM Addresses WHERE OrganisationId = O.Id AND CountryCode = '{request.Criteria.CountryCode}' AND Active = 1)";
                }

                string sQuery = $@";WITH cte_partnership_shipment AS (
                                        SELECT P.Id [PartnershipId], P.CreationDateInternal, P.ProviderId, P.ClientId, COUNT(S.Id) [ActiveShipments], MAX(Fm.CreationDateInternal) [LastFreightMovementEntered], MAX(S.BookedDate) [LastShipmentBooked], C.ContactEmail, C.Name, 1 [ConfirmedUsers]
                                        FROM [dbo].Partnerships P
                                            LEFT JOIN [dbo].Shipments S ON S.PartnershipId = P.Id AND S.Active = 1 AND S.Delivered = 0
                                            LEFT JOIN [dbo].FreightMovements FM ON FM.Id = S.FreightMovementId
                                            LEFT JOIN [dbo].[Organisations] C ON C.Id = P.{request.CompanyType} AND C.Active = 1
                                        WHERE P.{request.RequestingType} = @organisationId AND P.Active = 1 {where_clause.ToString()}
                                        GROUP BY P.Id, P.ProviderId, P.ClientId, P.CreationDateInternal, C.ContactEmail, C.Name
                                        {pendingQueryBuilder.ToString()}
                                    )
                                    SELECT O.Id [OrganisationId], PS.Name, PS.ContactEmail, PS.PartnershipId, PS.ActiveShipments, PS.CreationDateInternal, PS.[LastFreightMovementEntered], PS.LastShipmentBooked, PS.ConfirmedUsers
                                     FROM cte_partnership_shipment PS
                                        LEFT JOIN [dbo].[Organisations] O ON O.Id = PS.{request.CompanyType}
                                        WHERE {where_confirmed_users}
                                    ORDER BY PS.ActiveShipments desc, PS.Name
                                    
                                    SELECT O.Id [OrganisationId], A.CountryCode, C.Name [CountryName]
                                    FROM [dbo].[Partnerships] P
                                        LEFT JOIN [dbo].[Organisations] O ON O.Id = P.{request.CompanyType} AND O.Active = 1
                                        LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.Id AND A.Active = 1
                                        LEFT JOIN [dbo].[Countries] C ON C.Code2 = A.CountryCode
                                    WHERE A.Id IS NOT NULL AND P.{request.RequestingType} = @organisationId AND P.Active = 1
                ";


                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    using (var multi = await conn.QueryMultipleAsync(sQuery, new
                    {
                        request.OrganisationId,
                        @search = BuildSearchParam(request.Criteria.Search)
                    }))
                    {
                        List<LogisticsClient> organisation_query = (await multi.ReadAsync<LogisticsClient>())
                            .Where(l => l.ContactEmail != "logistics@tradefact.com")
                            .ToList();

                        List<LogisticsClientCountryResource> network_country_items = (await multi.ReadAsync<LogisticsClientCountryResource>()).ToList();

                        IPagedList<LogisticsClient> clients;
                        if (request.Criteria.OrderByName) clients = await organisation_query.OrderBy(x => x.Name).ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                        else clients = await organisation_query.OrderByDescending(x => x.CreationDateInternal).ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);

                        foreach (var item in clients)
                        {
                            var countryList = network_country_items
                                .Where(q => q.OrganisationId == item.OrganisationId)
                                .GroupBy(m => new { m.CountryCode, m.CountryName })
                                .Select(g => g.First())
                                .ToList();

                            item.CountryInfo = countryList.Adapt<List<OrganisationCountryResource>>();
                        }

                        return clients;
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
