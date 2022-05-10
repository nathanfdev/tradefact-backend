using Core.Common;
using Core.Models;
using Core.Enums;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using X.PagedList;

namespace Tradefact.Application.Network 
{
    public class GetNetworkListQuery : IRequest<IPagedList<DirectoryResource>>
    {
        public Guid? OrganisationId { get; set; }
        public string Search { get; set; }
        public PagedResultParameters Paging { get; set; }

        public bool IncludeOrders { get; set; }

        public string CountryFilter { get; set; }
        public ConnectionStatusEnum? StatusFilter { get; set; }
        public SourceEnum? SourceFilter { get; set; }

        public class GetNetworkListQueryHandler : IRequestHandler<GetNetworkListQuery, IPagedList<DirectoryResource>>
        {
            private readonly IDbConnection _connection;

            public GetNetworkListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<DirectoryResource>> Handle(GetNetworkListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder sqlBuilder = new StringBuilder();

                var extendedWhere = request.StatusFilter switch
                {
                    ConnectionStatusEnum.Pending => "AND S.Source = 3 ",
                    ConnectionStatusEnum.Active => "AND S.Source != 3 ",
                    _ => ""
                };

                if (request.CountryFilter != null)
                {
                    extendedWhere += $"AND EXISTS (SELECT * FROM Addresses WHERE OrganisationId = S.NetworkId AND CountryCode = '{request.CountryFilter}' AND Active = 1) ";
                }

                if (request.SourceFilter != null)
                {
                    extendedWhere += $"AND S.Source = {(int)request.SourceFilter} ";
                }

                if (request.IncludeOrders)
                {
                    sqlBuilder.AppendLine(@$"
                        ;WITH cte_network AS
                        (
                            SELECT 1 [Source], O.ParentId, O.Id [NetworkId], '' [Status], COUNT(DISTINCT A.Id) [Locations], O.Name, NULL [ContactEmail], O.CreationDateInternal, COUNT(DISTINCT C.Id) [Contacts]
                            FROM [dbo].[Organisations] O 
                                LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.id AND A.Active = 1
                                LEFT JOIN [dbo].[Contacts] C ON C.OrganisationId = O.id AND C.Active = 1
                            WHERE O.Active = 1 AND O.ParentId = @organisationId
                            GROUP BY O.ParentId, O.Id, O.Name, O.CreationDateInternal
                            UNION
                            SELECT 2 [Source], @organisationId [ParentId], B2B.LinkedOrganisationId [NetworkId], B2B.ConnectionStatus [Status], COUNT(DISTINCT A.Id) [Locations], O.Name, NULL [ContactEmail], O.CreationDateInternal, COUNT(DISTINCT C.Id) + COUNT(DISTINCT U.Id) [Contacts]
                            FROM [dbo].[B2B_Connections] B2B
                                INNER JOIN [dbo].[Organisations] O ON O.Id = B2B.LinkedOrganisationId
                                LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.id AND A.Active = 1
                                LEFT JOIN [dbo].[B2B_ConnectionContacts] C ON C.ConnectionId = B2B.Id AND C.Active = 1
                                LEFT JOIN [dbo].[AspNetUsers] U ON U.OrganisationId = B2B.LinkedOrganisationId AND U.Status = 'Active' AND U.IsExposedToOtherOrganization = 1
                            WHERE B2B.OrgansationId = @organisationId and B2B.Active = 1 AND B2B.ConnectionStatus = 1
                            GROUP BY B2B.LinkedOrganisationId, B2B.ConnectionStatus, O.Name, O.CreationDateInternal
                            UNION
                            SELECT 3 [Source], @organisationId [ParentId], NULL [NetworkId], 0 [Status], 0 [Locations], AI.CompanyName [Name], AI.EmailAddress [ContactEmail], AI.CreationDateInternal, 0 [Contacts]
                                FROM [dbo].[AspNetUserInvitations] AI
                                LEFT JOIN [dbo].AspNetUsers AU ON AU.Email = AI.EmailAddress
                            WHERE AI.InviteRequestedByOrganisationId = @organisationId AND AI.ActivationStatus != 2 and AI.Active = 1 AND AI.InviteType = 2 and AU.Id IS NULL
                        ), 
                        cte_orders AS 
                        (
                            SELECT CS.Source, CS.ParentId, CS.NetworkId, CS.Locations, CS.Status, SUM(CASE WHEN PO.Id IS NULL THEN 0 ELSE 1 END) [ActiveOrders], CS.Name, CS.ContactEmail, CS.CreationDateInternal, CS.Contacts
                            FROM cte_network CS
                                LEFT JOIN PurchaseOrders PO ON ((PO.SupplierId = CS.NetworkId AND PO.CompanyId = @organisationId) OR (PO.SupplierId = @organisationId AND PO.CompanyId = CS.NetworkId)) AND PO.Active = 1 AND PO.[Status] IN (20,30,40,50) 
                            GROUP BY CS.Source, CS.ParentId, CS.NetworkId, CS.Locations, CS.Status, CS.Name, CS.ContactEmail, CS.CreationDateInternal, CS.Contacts
                        ),
                        cte_organisation_orders AS
                        (
                            SELECT CO.Source, CO.ParentId, CO.NetworkId, CO.[ActiveOrders], CO.Status, [Locations], CO.Name, CO.ContactEmail, CO.CreationDateInternal, SUM(CASE WHEN S.ID IS NULL THEN 0 ELSE 1 END) [ActiveShipments], CO.Contacts
                            FROM cte_orders CO
                                LEFT JOIN [dbo].FreightMovements FM ON (FM.SupplierId = CO.NetworkId OR FM.BuyerId = CO.NetworkId)
                                LEFT JOIN [dbo].Shipments S ON S.FreightMovementId = FM.Id AND (S.Active = 1 AND ISNULL(S.Delivered,0) = 0)
                            GROUP BY CO.Source, CO.ParentId, CO.NetworkId, CO.ActiveOrders, CO.Status, [Locations], CO.Name, CO.ContactEmail, CO.CreationDateInternal, CO.Contacts
                        )
                        SELECT S.Source, S.ActiveOrders, S.Name, O.ContactName, O.Currency, O.CreationDateInternal, S.ContactEmail, O.ContactTelephone, O.PaymentTerms, O.TCs, S.Status, S.Locations, S.ActiveShipments, S.ParentId, S.NetworkId [Id], S.Contacts
                        FROM cte_organisation_orders S
                            LEFT JOIN [dbo].[Organisations] O  ON O.Id = S.NetworkId
                            WHERE (S.Name like @search OR @search is NULL) {extendedWhere}
                            ORDER BY S.CreationDateInternal desc

                        SELECT O.Id [OrganisationId], A.CountryCode, C.Name [CountryName]
                        FROM [dbo].[Organisations] O
                            LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.Id AND A.Active = 1
                            LEFT JOIN [dbo].[Countries] C ON C.Code2 = A.CountryCode
                        WHERE A.Id IS NOT NULL
                        GROUP BY O.Id, A.CountryCode, A.Id, C.Name
                   ");
                } else
                {
                    sqlBuilder.AppendLine($@"
                    SELECT DT.[Source], DT.ParentId, DT.NetworkId [Id], ISNULL(DT.[Status], 1) [Status], DT.[Name], DT.CreationDateInternal, DT.Currency
                      FROM (
                        SELECT 1 [Source], O.ParentId, O.Id [NetworkId], '' [Status], COUNT(A.Id) [Locations], O.Name, NULL [ContactEmail], O.CreationDateInternal, ISNULL(O.Currency, 'USD') [Currency]
                        FROM [dbo].[Organisations] O 
                            LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.id AND A.Active = 1
                        WHERE O.Active = 1 AND O.ParentId = @organisationId
                        GROUP BY O.ParentId, O.Id, O.Name, O.CreationDateInternal, ISNULL(O.Currency, 'USD')
                        UNION
                        SELECT 2 [Source], @organisationId [ParentId], B2B.LinkedOrganisationId [NetworkId], B2B.ConnectionStatus [Status], COUNT(A.Id) [Locations], O.Name, NULL [ContactEmail], O.CreationDateInternal, ISNULL(O.Currency, 'USD') [Currency]
                        FROM [dbo].[B2B_Connections] B2B
                            INNER JOIN [dbo].[Organisations] O ON O.Id = B2B.LinkedOrganisationId
                            LEFT JOIN [dbo].[Addresses] A ON A.OrganisationId = O.id AND A.Active = 1
                        WHERE B2B.OrgansationId = @organisationId and B2B.Active = 1 AND B2B.ConnectionStatus = 1
                        GROUP BY B2B.LinkedOrganisationId, B2B.ConnectionStatus, O.Name, O.CreationDateInternal, ISNULL(O.Currency, 'USD')
                       ) DT
                    ORDER BY DT.Name, DT.CreationDateInternal desc");
                }

                string sQuery = sqlBuilder.ToString();

                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    if (request.IncludeOrders)
                    {
                        using (var multi = await conn.QueryMultipleAsync(sQuery, new
                        {
                            @organisationId = request.OrganisationId,
                            @search = BuildSearchParam(request.Search)
                        }))
                        {
                            List<ConnectionInfo> network_items_query = (await multi.ReadAsync<ConnectionInfo>()).ToList();

                            List<DirectoryCountryResource> network_country_items = (await multi.ReadAsync<DirectoryCountryResource>()).ToList();

                            List<DirectoryResource> network_items = network_items_query.Adapt<List<DirectoryResource>>();

                            if (request.Search != null && request.Search.Length > 0)
                            {
                                IQueryable<SortedDirectoryResource> network_query = network_items.Adapt<List<SortedDirectoryResource>>().AsQueryable();
                                IPagedList<DirectoryResource> networkSearch = await network_query.Adapt<List<SortedDirectoryResource>>().OrderBy(x => x.Position(request.Search)).ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                                var searchResult = HandleCountryList(networkSearch, network_country_items);
                                return searchResult;
                            }

                            IPagedList<DirectoryResource> networkResult = await network_items.Adapt<List<DirectoryResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                            var result = HandleCountryList(networkResult, network_country_items);

                            return result;
                        }

                    }

                    IEnumerable<ConnectionInfo> query = await conn.QueryAsync<ConnectionInfo>(sQuery, new
                    {
                        @organisationId = request.OrganisationId,
                        @search = BuildSearchParam(request.Search)
                    });

                    if (request.Search != null && request.Search.Length > 0)
                    {
                        IQueryable<SortedDirectoryResource> network_query = query.Adapt<List<SortedDirectoryResource>>().AsQueryable();
                        IPagedList<DirectoryResource> networkSearch = await network_query.Adapt<List<SortedDirectoryResource>>().OrderBy(x => x.Position(request.Search)).ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                        return networkSearch;
                    }

                    IPagedList<DirectoryResource> network = await query.Adapt<List<DirectoryResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                    return network;
                }
            }

            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }

            private IPagedList<DirectoryResource> HandleCountryList(IPagedList<DirectoryResource> network, List<DirectoryCountryResource> network_country_items)
            {
                foreach (var item in network)
                {
                    var countryList = network_country_items
                        .Where(q => q.OrganisationId == item.Id)
                        .GroupBy(m => new { m.CountryCode, m.CountryName })
                        .Select(g => g.First())
                        .ToList();

                    item.CountryInfo = countryList.Adapt<List<OrganisationCountryResource>>();
                }

                return network;
            }
        }

    }
}
