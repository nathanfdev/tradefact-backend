using Dapper;
using Mapster;
using MediatR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Core.Enums;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderProductDocumentsQuery : IRequest<List<PurchaseOrderProductDocumentResource>>
    {
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid ProductId { get; set; }
        public bool AttachedOnly { get; set; }
        public ProductDocumentType DocumentType;

        public class GetPurchaseOrderProductDocumentsQueryHandler : IRequestHandler<GetPurchaseOrderProductDocumentsQuery, List<PurchaseOrderProductDocumentResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderProductDocumentsQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<List<PurchaseOrderProductDocumentResource>> Handle(GetPurchaseOrderProductDocumentsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder extended_where = new StringBuilder();
                if (request.AttachedOnly) {
                    extended_where.Append(" AND PAD.PurchaseOrderProductId IS NOT NULL");
                }
                if (request.DocumentType != ProductDocumentType.All)
                {
                    extended_where.Append($" AND PD.DocumentType = {(int)request.DocumentType}");
                }
                string sQuery = @$";WITH cte_products AS (
  SELECT [Id], 0 [IsProductVariant] 
    FROM [dbo].[Products] P 
  WHERE P.Id = @ProductId
 UNION
  SELECT [Id], 1 [IsProductVariant] 
    FROM [dbo].[ProductVariants] PV 
  WHERE PV.Id = @ProductId
 UNION
  SELECT P.[Id], 0 [IsProductVariant] 
    FROM [dbo].[ProductVariants] PV 
    INNER JOIN [dbo].[Products] P ON P.Id = PV.ProductId
  WHERE PV.Id = @ProductId
) 
SELECT  PD.ProductId [ProductId], D.Id [DocumentId], D.[Description], D.Name, D.Extension, 
        D.BlobUrl, PD.DocumentType, D.IsRichText, CASE WHEN PAD.PurchaseOrderProductId IS NULL THEN 0 ELSE 1 END [Attached], [IsProductVariant] [IsProductVariantDocument] 
FROM cte_products P
    INNER JOIN [dbo].[ProductDocuments] PD ON PD.ProductId = P.id
    INNER JOIN [Documents] D ON D.Id = PD.DocumentId
    LEFT JOIN [dbo].[PurchaseOrderAttachedProductDocuments] PAD ON PAD.PurchaseOrderProductId = PD.ProductId 
        and PAD.PurchaseOrderId = @PurchaseOrderId and PAD.DocumentId = PD.DocumentId
WHERE PD.IsActive = 1 {extended_where.ToString()};";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    var result = await conn.QueryAsync<PurchaseOrderProductDocumentResource>(sQuery, new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId,
                        @ProductId = request.ProductId
                    });
                    return result.ToList();
                }

            }
        }
    }

}
