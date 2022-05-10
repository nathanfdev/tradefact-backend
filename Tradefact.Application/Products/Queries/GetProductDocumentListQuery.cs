using Core.Dtos;
using Core.Enums;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Application.Products.Queries
{
    public class GetProductDocumentListQuery : IRequest<IPagedList<DocumentInfo>>
    {
        public Guid ProductId { get; set; }

        public ProductDocumentType DocumentType { get; set; }

        public string Search { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string UserEmail { get; set; }
        public bool? IsRichText { get; set; }

        public class GetProductDocumentListQueryHandler : IRequestHandler<GetProductDocumentListQuery, IPagedList<DocumentInfo>>
        {
            private readonly TradefactDbContext _context;

            public GetProductDocumentListQueryHandler(TradefactDbContext context)
            {
                _context = context;
            }

            public async Task<IPagedList<DocumentInfo>> Handle(GetProductDocumentListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                IQueryable<ProductDocumentResource> query = _context.ProductDocuments
                    .Include(i => i.Document)
                    .Where(q => 
                        q.ProductId == request.ProductId && 
                        q.IsActive && 
                        (request.IsRichText == null || q.Document.IsRichText == request.IsRichText) && 
                        q.DocumentType == request.DocumentType)
                    .Select(s => new ProductDocumentResource
                    {
                        Id = s.DocumentId,
                        BlobUrl = s.Document.BlobUrl,
                        ThumbnailUrl = s.Document.ThumbnailGenerated ? s.Document.ThumbnailUrl : s.Document.BlobUrl,
                        Name = s.Document.Name,
                        Extension = s.Document.Extension,
                        Description = s.Document.Description,
                        DateUploaded = s.Document.DateUploaded,
                        IsRichText = s.Document.IsRichText,
                        RichTextData = s.Document.RichTextData,
                        UploadedBy = s.Document.UploadedBy,
                        DocumentType = (int)s.DocumentType,
                        CreationDateInternal = s.Document.CreationDateInternal,
                        IsDefaultImage = s.IsDefaultImage,
                        IsActive = s.IsActive,
                        CreatedByUser = s.Document.CreatedByUser
                    })
                    .OrderByDescending(x => x.CreationDateInternal);

                if (request.Search != null && request.Search.Length > 1)
                {
                    query = query.Where(q => EF.Functions.Like(q.Description, $"%{request.Search}%") || EF.Functions.Like(q.Name, $"%{request.Search}%"));
                }
                IPagedList<DocumentInfo> documents = await query.Adapt<List<DocumentInfo>>().ToPagedListAsync(request.PageNumber, request.PageSize);
                foreach (DocumentInfo doc in documents)
                {
                    doc.Owner = doc.CreatedByUser == request.UserEmail;
                }
                return documents;
            }
        }
    }
}
