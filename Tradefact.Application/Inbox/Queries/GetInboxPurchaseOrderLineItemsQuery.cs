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

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetInboxPurchaseOrderLineItemsQuery : IRequest<List<InboxPurchaseOrderLineItemResource>>
    {
        public Guid OrganisationId { get; set; }
        public Guid ExternalPurchaseOrderId { get; set; }

        public class GetInboxPurchaseOrderLineItemsQueryHandler : IRequestHandler<GetInboxPurchaseOrderLineItemsQuery, List<InboxPurchaseOrderLineItemResource>>
        {
            private readonly IDbConnection _connection;

            public GetInboxPurchaseOrderLineItemsQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<List<InboxPurchaseOrderLineItemResource>> Handle(GetInboxPurchaseOrderLineItemsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.AppendLine(@"
                    ;WITH cte_product_sku AS (
                        SELECT xli.[Id], p.[Id] [ProductId], ISNULL(p.[DataComplete], 0) [ProductDataComplete], 
                            CASE WHEN (p.[SKU] IS NULL AND xpo.[GenericProductId] IS NULL) THEN 0 ELSE 1 END [ExistsInCatalogue],
                            RANK() OVER (PARTITION BY xli.Id ORDER BY p.[LastModifiedOnInternal]) [Rank]
                        FROM [integration].[ExternalPurchaseOrderLineItems] xli
                            JOIN [integration].[ExternalPurchaseOrders] xpo ON xpo.[Id] = xli.[ExternalPurchaseOrderId]
                            LEFT OUTER JOIN [dbo].[Products] p ON p.[SKU] = xli.SKU AND p.CompanyId = @organisationId AND p.Active = 1
                        WHERE xpo.[Id] = @externalPurchaseOrderId
                    )
                    SELECT xli.[Id], xli.[SKU], xli.[Description], xli.[Quantity], xpo.[CurrencyCode], xli.[LineAmount], xli.[Tags], 
                        cps.[ProductId], cps.[ExistsInCatalogue], cps.[ProductDataComplete]
                        FROM [integration].[ExternalPurchaseOrderLineItems] xli
                            JOIN [integration].[ExternalPurchaseOrders] xpo ON xpo.[Id] = xli.[ExternalPurchaseOrderId]
                            JOIN cte_product_sku cps ON cps.Id = xli.[Id] AND cps.[Rank] = 1
                        WHERE xpo.[OrganisationId] = @organisationId
                            AND xli.[ExternalPurchaseOrderId] = @externalPurchaseOrderId
                            AND xli.[Active] = 1");

                string sQuery = queryBuilder.ToString();

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    IEnumerable<InboxPurchaseOrderLineItemResource> result = await conn.QueryAsync<InboxPurchaseOrderLineItemResource>(sQuery, new
                    {
                        @OrganisationId = request.OrganisationId,
                        @ExternalPurchaseOrderId = request.ExternalPurchaseOrderId
                    });

                    var purchaseOrders = result.Adapt<List<InboxPurchaseOrderLineItemResource>>();
                    return purchaseOrders;
                }
            }
        }
    }
}
