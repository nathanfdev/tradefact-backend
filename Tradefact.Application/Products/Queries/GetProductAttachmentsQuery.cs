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
using Core.Dtos;
using Core.Enums;

namespace Tradefact.Application.Document.Queries
{
    public class GetProductAttachmentsQuery : IRequest<List<DocumentInfo>>
    {
        public Guid ProductId { get; set; }

        public string UserEmail { get; set; }

        public class GetProductAttachmentsQueryHandler : IRequestHandler<GetProductAttachmentsQuery, List<DocumentInfo>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetProductAttachmentsQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<List<DocumentInfo>> Handle(GetProductAttachmentsQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string sQuery = @$";WITH cte_products AS (
                                      SELECT [Id], 0 [IsProductVariant], 'Catalogue' [Src]
                                        FROM [dbo].[Products] P 
                                      WHERE P.Id = @ProductId
                                     UNION
                                      SELECT [Id], 1 [IsProductVariant], 'Variant' [Src]
                                        FROM [dbo].[ProductVariants] PV 
                                      WHERE PV.Id = @ProductId
                                     UNION
                                      SELECT P.[Id], 0 [IsProductVariant], 'Parent' [Src]
                                        FROM [dbo].[ProductVariants] PV 
                                        INNER JOIN [dbo].[Products] P ON P.Id = PV.ProductId
                                      WHERE PV.Id = @ProductId
                                    ) 
                                    SELECT P.[Src], PD.ProductId, P.IsProductVariant, CAST(CASE WHEN PD.ProductId = @ProductId AND D.CreatedByUser = @UserEmail THEN 1 ELSE 0 END as BIT) [Owner],
                                     D.Id, D.DateUploaded, D.[Description], D.Extension, D.Name, IsRichText, RichTextData, D.BlobUrl
                                     FROM cte_products P
                                        INNER JOIN [dbo].[ProductDocuments] PD  ON PD.ProductId = P.Id    
                                        INNER JOIN [dbo].[Documents] D ON D.Id = PD.DocumentId
                                    WHERE PD.IsActive = 1 AND D.Active = 1 AND PD.DocumentType = 1
                                    ORDER BY D.IsRichText, P.IsProductVariant, D.DateUploaded";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    var result = await conn.QueryAsync<DocumentInfo>(sQuery, new
                    {
                        @ProductId = request.ProductId,
                        @UserEmail = request.UserEmail
                    });
                    return result.ToList();
                }

            }
        }
    }

}
