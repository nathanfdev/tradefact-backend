using Core.Enums;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderDocumentsQuery : IRequest<List<PurchaseOrderProductDocumentResource>>
    {
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public string UserEmail { get; set; }
        public ProductDocumentType DocumentType;

        public class GetPurchaseOrderDocumentsQueryHandler : IRequestHandler<GetPurchaseOrderDocumentsQuery, List<PurchaseOrderProductDocumentResource>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetPurchaseOrderDocumentsQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<List<PurchaseOrderProductDocumentResource>> Handle(GetPurchaseOrderDocumentsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                StringBuilder extended_where = new StringBuilder();
                if (request.DocumentType != ProductDocumentType.All)
                {
                    extended_where.Append($" AND PD.DocumentType = {(int)request.DocumentType}");
                }
                string sQuery = @$"SELECT PAD.PurchaseOrderProductId [ProductId], D.Id [DocumentId], D.[Description], 
                                    D.Name, D.Extension, PD.DocumentType, D.BlobUrl, D.IsRichText, D.DateUploaded, D.UploadedBy,
                                    CASE WHEN PAD.PurchaseOrderProductId IS NULL THEN 0 ELSE 1 END [Attached],
                                    CASE WHEN D.CreatedByUser = @UserEmail THEN 1 ELSE 0 END [Owner]
                                        FROM [dbo].[PurchaseOrderAttachedProductDocuments] PAD 
                                            INNER JOIN [Documents] D ON D.Id = PAD.DocumentId
                                            INNER JOIN [ProductDocuments] PD on PD.DocumentId = PAD.DocumentId AND PD.ProductId = PAD.PurchaseOrderProductId
                                    WHERE PAD.PurchaseOrderId = @PurchaseOrderId AND PAD.IsActive = 1 {extended_where.ToString()};";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    var result = await conn.QueryAsync<PurchaseOrderProductDocumentResource>(sQuery, new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId,
                        @UserEmail = request.UserEmail
                    });
                    return result.ToList();
                }

            }
        }
    }

}
