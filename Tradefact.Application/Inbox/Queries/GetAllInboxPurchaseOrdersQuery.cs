using Core.Common;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Models;
using X.PagedList;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetAllInboxPurchaseOrdersQuery : IRequest<IPagedList<InboxPurchaseOrderResource>>
    {
        public Guid OrganisationId { get; set; }
        public InboxPurchaseOrderSearchCriteria Criteria { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetAllInboxPurchaseOrdersQueryHandler : IRequestHandler<GetAllInboxPurchaseOrdersQuery, IPagedList<InboxPurchaseOrderResource>>
        {
            private readonly IDbConnection _connection;

            public GetAllInboxPurchaseOrdersQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<InboxPurchaseOrderResource>> Handle(GetAllInboxPurchaseOrdersQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.AppendLine(@"
                    ;WITH cte_sku_count AS (
                      SELECT o.[Id], COUNT(i.[Id]) - COUNT(p.SKU) [SKUNotFoundCount], COUNT(CASE WHEN p.[DataComplete] = 0 THEN 1 END) [IncompleteProductCount]
                       FROM [integration].[ExternalPurchaseOrders] o
                            LEFT OUTER JOIN [integration].[ExternalPurchaseOrderLineItems] i ON i.[ExternalPurchaseOrderId] = o.[Id]
                            LEFT OUTER JOIN [dbo].[Products] p ON p.[SKU] = i.SKU AND p.CompanyId = @organisationId AND p.Active = 1
                            WHERE o.OrganisationId = @organisationId
                            AND o.[Imported] = 0
                            AND o.[Active] = 1 
                            GROUP BY o.[Id]
                    ),
                    cte AS (
                      SELECT o.[Id], COUNT(i.[Id]) [NoOfLines], cs.[SKUNotFoundCount], cs.[IncompleteProductCount]
                        FROM [integration].[ExternalPurchaseOrders] o
                        LEFT OUTER JOIN [integration].[ExternalPurchaseOrderLineItems] i ON i.[ExternalPurchaseOrderId] = o.[Id]
                        INNER JOIN cte_sku_count cs ON cs.Id = o.[Id]
                      GROUP BY o.[Id], cs.[SKUNotFoundCount], cs.[IncompleteProductCount])
                    SELECT DISTINCT xpo.[Id], xpo.[PurchaseOrderNumber], xpo.[Reference], xpo.[OrderDate], xpo.[Status], 
                      xpo.[SupplierName], xpo.SupplierId, xpo.[Tags],
                      CASE WHEN po.[PurchaseOrderNumber] IS NULL THEN 0 ELSE 1 END [IsDuplicatePONumber], 
                      cte.[NoOfLines], cte.[SKUNotFoundCount], cte.[IncompleteProductCount], xpo.[GenericProductId], xpo.Source, xpo.[CreationDateInternal]
                    FROM cte
                      JOIN [integration].[ExternalPurchaseOrders] xpo ON xpo.Id = cte.Id
                      LEFT OUTER JOIN [dbo].[PurchaseOrders] po ON po.[CompanyId] = @organisationId AND po.[PurchaseOrderNumber] = xpo.[PurchaseOrderNumber]
                ");

                if (request.Criteria?.OrderNo != null)
                {
                    queryBuilder.AppendLine($"AND xpo.[PurchaseOrderNumber] LIKE '%{request.Criteria.OrderNo}%'");
                }

                if (request.Criteria?.SupplierName != null)
                {
                    queryBuilder.AppendLine($"AND xpo.[SupplierName] LIKE '%{request.Criteria.SupplierName}%'");
                }

                queryBuilder.AppendLine($"ORDER BY xpo.[CreationDateInternal] desc");

                string sQuery = queryBuilder.ToString();

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    IEnumerable<InboxPurchaseOrderResource> result = await conn.QueryAsync<InboxPurchaseOrderResource>(sQuery, new
                    {
                        @OrganisationId = request.OrganisationId
                    });

                    var purchaseOrders = await result.Adapt<List<InboxPurchaseOrderResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                    return purchaseOrders;
                }
            }
        }
    }
}
