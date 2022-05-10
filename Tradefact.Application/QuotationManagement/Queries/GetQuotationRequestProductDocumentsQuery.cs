using Core.Dtos;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Tradefact.Application.QuotationManagement.Queries
{
    public class GetQuotationRequestProductDocumentsQuery : IRequest<List<DocumentInfo>>
    {
        public Guid QuotationRequestId { get; set; }
        public string Search { get; set; }


        public class GetPurchaseOrderProductDocumentsQueryHandler : IRequestHandler<GetQuotationRequestProductDocumentsQuery, List<DocumentInfo>>
        {
            private readonly IDbConnection _connection;

            public GetPurchaseOrderProductDocumentsQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
            }

            public async Task<List<DocumentInfo>> Handle(GetQuotationRequestProductDocumentsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string extendedWhere = null;
                if (request.Search != null)
                {
                    extendedWhere = $"AND (d.[Name] LIKE '%{request.Search}%' OR d.[Description] LIKE '%{request.Search}%')";
                }

                string sQuery = @$";
SELECT d.*, pd.[DocumentType], pd.[IsDefaultImage], pd.[IsActive]
FROM [dbo].[QuotationRequests] qr 
JOIN [dbo].[FreightMovements] fm ON fm.[Id] = qr.[FreightMovementId]
JOIN [dbo].[FreightMovementItems] fmi ON fmi.[FreightMovementId] = fm.[Id]
JOIN [dbo].[PurchaseOrderItemScheduleLines] poisl ON poisl.[Id] = fmi.[PurchaseOrderItemScheduleLineId]
JOIN [dbo].[PurchaseOrderItems] poi ON poi.[Id] = poisl.[PurchaseOrderItemId]
JOIN [dbo].[PurchaseOrderAttachedProductDocuments] poapd ON poapd.[PurchaseOrderId] = poi.[PurchaseOrderId] AND poapd.[PurchaseOrderProductId] = poi.[ProductId]
JOIN [dbo].[ProductDocuments] pd ON pd.ProductId = poapd.[PurchaseOrderProductId] AND pd.[DocumentId] = poapd.[DocumentId]
JOIN [dbo].[Documents] d ON d.Id = pd.[DocumentId]
WHERE qr.[Id] = @QrId AND pd.[IsActive] = 1 {extendedWhere};";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    var result = await conn.QueryAsync<DocumentInfo>(sQuery, new
                    {
                        @QrId = request.QuotationRequestId
                    });

                    return result.ToList();
                }
            }
        }
    }
}
