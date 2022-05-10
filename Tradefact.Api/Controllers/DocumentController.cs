using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Common;
using Core.Dtos;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using Tradefact.Api.Model;
using Tradefact.Api.Model.Document;
using Tradefact.Application.Document.Queries;
using Tradefact.Application.Models;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Application.QuotationManagement.Queries;
using Tradefact.Data;
using Tradefact.Services;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : FilesController
    {
        protected readonly IMediator _mediator;
        private readonly ITradefactActivityService _tradefactActivityService;

        public DocumentController(ILogger<DocumentController> logger, IMediator mediator, TradefactDbContext context, IAzureBlobService blobService, IMimeMappingService mimeMappingService, ITradefactActivityService tradefactActivityService) : base(logger, context, blobService, mimeMappingService) {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tradefactActivityService = tradefactActivityService;
        }

        protected override string GetBlobPath()
        {
            return $"documents";
        }

        public override string[] PermittedExtensions => this.PermittedImageExtensions.Concat(this.PermittedDocumentExtensions).ToArray();

        public async Task<IActionResult> Get(Guid id)
        {

            Document doc = await _context.Documents.SingleOrDefaultAsync(s=>s.Id == id);

            string containerName = doc.CompanyId.ToString("N");
            var blob = _blobService.GetBlockBlob(containerName, doc.BlobUrl);
            var blobExists = await blob.ExistsAsync();


            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            Stream blobStream = await blob.OpenReadAsync();
            return File(blobStream, blob.Properties.ContentType, doc.Name);
        }

        [HttpDelete]
        [Route("deleteproductdocument")]
        public async Task<IActionResult> DeleteProductDocument(Guid documentId, Guid productId)
        {
            Document doc = await _context.Documents.SingleOrDefaultAsync(s => s.Id == documentId);
            ProductDocument proddoc = await _context.ProductDocuments.SingleOrDefaultAsync(s => s.DocumentId == documentId && s.ProductId == productId);
            proddoc.IsActive = false;
            if (proddoc.IsDefaultImage)
            {
                proddoc.IsDefaultImage = false;
            }
            _ = await _context.SaveChangesAsync();

            if(!doc.IsRichText)
            {
                return Ok(doc.Adapt<DocumentInfo>());
            }
            else
            {
                DocumentInfo docInfo = new DocumentInfo() 
                {
                    Name = doc.Name,
                    Description = doc.Description,
                    DateUploaded = doc.DateUploaded,
                    Id = doc.Id,
                    IsActive = doc.IsActive,
                    IsRichText = doc.IsRichText,
                    RichTextData = doc.RichTextData,
                    UploadedBy = doc.UploadedBy
                };
                return Ok(docInfo);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            Document doc = await _context.Documents.SingleOrDefaultAsync(s => s.Id == id);
            ShipmentDocument shipdoc = await _context.ShipmentDocuments.SingleOrDefaultAsync(s => s.DocumentId == id);
            shipdoc.IsActive = false;
            _ = await _context.SaveChangesAsync();

            if (!doc.IsRichText)
            {
                return Ok(doc.Adapt<DocumentInfo>());
            }
            else
            {
                DocumentInfo docInfo = new DocumentInfo()
                {
                    Name = doc.Name,
                    Description = doc.Description,
                    DateUploaded = doc.DateUploaded,
                    Id = doc.Id,
                    IsActive = doc.IsActive,
                    IsRichText = doc.IsRichText,
                    RichTextData = doc.RichTextData,
                    UploadedBy = doc.UploadedBy
                };
                return Ok(docInfo);
            }
        }

        [HttpDelete]
        [Route("purchaseorderdocument")]
        public async Task<IActionResult> DeletePODocument(Guid id)
        {
            Document doc = await _context.Documents.SingleOrDefaultAsync(s => s.Id == id);
            PurchaseOrderDocument podoc = await _context.PurchaseOrderDocuments.SingleOrDefaultAsync(s => s.DocumentId == id);
            podoc.IsActive = false;
            _ = await _context.SaveChangesAsync();

            if (!doc.IsRichText)
            {
                return Ok(doc.Adapt<DocumentInfo>());
            }
            else
            {
                DocumentInfo docInfo = new DocumentInfo()
                {
                    Name = doc.Name,
                    Description = doc.Description,
                    DateUploaded = doc.DateUploaded,
                    Id = doc.Id,
                    IsActive = doc.IsActive,
                    IsRichText = doc.IsRichText,
                    RichTextData = doc.RichTextData,
                    UploadedBy = doc.UploadedBy
                };
                return Ok(docInfo);
            }
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("shipment")]
        public async Task<IActionResult> GetShipmentDocuments([FromQuery] PagedResultParameters @params, Guid id, string search = null)
        {
            IQueryable<Document> query = _context.ShipmentDocuments.Include(i=>i.Document).Where(q=>q.ShipmentId == id && q.IsActive).Select(s=>s.Document);

            if (search != null && search.Length > 1)
            {
                query = query.Where(q => EF.Functions.Like(q.Description, $"%{search}%") || EF.Functions.Like(q.Name, $"%{search}%"));
            }
            IPagedList<DocumentInfo> documents = await query.Adapt<List<DocumentInfo>>().ToPagedListAsync(@params.PageNumber, @params.PageSize);
            foreach (DocumentInfo doc in documents)
            {
                doc.Owner = doc.CreatedByUser == GetUserEmail();
            }
            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(documents));
        }

        [HttpPost]
        [SwaggerResponse("201", typeof(List<DocumentInfo>), Description = "Created Result")]
        [DisableFormValueModelBinding]
        [Route("shipment")]
        public async Task<IActionResult> Shipment([FromQuery] Guid id)
        {
            UploadResult uploadResult = await this.ProcessUploadRequest($"{this.BlobPath}");
            if (!uploadResult.Success)
            {
                ModelStateDictionary state = (ModelStateDictionary)uploadResult;
                return BadRequest(state);
            }
            List<DocumentInfo> result = new List<DocumentInfo>();
            string userFullName = GetUserFullName();
            foreach (var item in uploadResult.Items)
            {
                Document doc = new Document { Id = Guid.NewGuid(), BlobUrl = item.BlobUrl, Name = item.OriginalFileName, CompanyId = this.OrganisationId, UploadedBy = userFullName };
                ShipmentDocument sd = new ShipmentDocument { ShipmentId = id, DocumentId = doc.Id };

                _context.Documents.Add(doc);
                _context.ShipmentDocuments.Add(sd);

                result.Add(doc.Adapt<DocumentInfo>());
            }

            _context.SaveChanges();
            return Created(nameof(DocumentController), result);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("purchaseorder")]
        public async Task<IActionResult> GetPurchaseOrderDocuments([FromQuery] PagedResultParameters @params, Guid id, string search = null)
        {
            IQueryable<PurchaseOrderDocumentResource> query = _context.PurchaseOrderDocuments.Include(i => i.Document).Where(q => q.PurchaseOrderId == id && q.IsActive).Select(s =>
                new PurchaseOrderDocumentResource
                {
                    Id = s.DocumentId,
                    BlobUrl = s.Document.BlobUrl,
                    Name = s.Document.Name,
                    Extension = s.Document.Extension,
                    Description = s.Document.Description,
                    DateUploaded = s.Document.DateUploaded,
                    IsRichText = s.Document.IsRichText,
                    RichTextData = s.Document.RichTextData,
                    DocumentType = (int)s.DocumentType,
                    UploadedBy = s.Document.UploadedBy
                });
            if (search != null && search.Length > 1)
            {
                query = query.Where(q => EF.Functions.Like(q.Description, $"%{search}%") || EF.Functions.Like(q.Name, $"%{search}%"));
            }
            IPagedList<DocumentInfo> documents = await query.OrderByDescending(d => d.DateUploaded).Adapt<List<DocumentInfo>>().ToPagedListAsync(@params.PageNumber, @params.PageSize);
            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(documents));
        }

        [HttpPost]
        [SwaggerResponse("201", typeof(List<DocumentInfo>), Description = "Created Result")]
        [DisableFormValueModelBinding]
        [Route("purchaseorder")]
        public async Task<IActionResult> PurchaseOrder([FromQuery] Guid id)
        {
            UploadResult uploadResult = await this.ProcessUploadRequest($"{this.BlobPath}");
            if (!uploadResult.Success)
            {
                ModelStateDictionary state = (ModelStateDictionary)uploadResult;
                return BadRequest(state);
            }
            List<DocumentInfo> result = new List<DocumentInfo>();
            string userFullName = GetUserFullName();
            foreach (var item in uploadResult.Items)
            {
                Document doc = new Document { Id = Guid.NewGuid(), BlobUrl = item.BlobUrl, Name = item.OriginalFileName, CompanyId = this.OrganisationId, UploadedBy = userFullName };
                PurchaseOrderDocument pd = new PurchaseOrderDocument { PurchaseOrderId = id, DocumentId = doc.Id, DocumentType = PurchaseOrderDocumentType.Default };

                _context.Documents.Add(doc);
                _context.PurchaseOrderDocuments.Add(pd);

                result.Add(doc.Adapt<DocumentInfo>());
            }

            _context.SaveChanges();

            _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(id, PurchaseOrderEventType.DocumentUploaded, DateTime.Now));
            return Created(nameof(DocumentController), result);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("purchaseorder/paymentterms")]
        public async Task<IActionResult> GetPurchaseOrderPaymentTermsDocuments([FromQuery] PagedResultParameters @params, Guid id, string search = null)
        {
            IQueryable<PurchaseOrderDocumentResource> query = _context.PurchaseOrderDocuments.Include(i => i.Document).Where(q => q.PurchaseOrderId == id && q.IsActive && q.DocumentType == PurchaseOrderDocumentType.PaymentTerms).Select(s => 
                new PurchaseOrderDocumentResource
                {
                    Id = s.DocumentId,
                    BlobUrl = s.Document.BlobUrl,
                    Name = s.Document.Name,
                    Extension = s.Document.Extension,
                    Description = s.Document.Description,
                    DateUploaded = s.Document.DateUploaded,
                    IsRichText = s.Document.IsRichText,
                    RichTextData = s.Document.RichTextData,
                    DocumentType = (int)s.DocumentType,
                    UploadedBy = s.Document.UploadedBy
                });
            if (search != null && search.Length > 1)
            {
                query = query.Where(q => EF.Functions.Like(q.Description, $"%{search}%") || EF.Functions.Like(q.Name, $"%{search}%"));
            }
            IPagedList<DocumentInfo> documents = await query.OrderByDescending(d => d.DateUploaded).Adapt<List<DocumentInfo>>().ToPagedListAsync(@params.PageNumber, @params.PageSize);
            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(documents));
        }

        [HttpPost]
        [SwaggerResponse("201", typeof(List<DocumentInfo>), Description = "Created Result")]
        [DisableFormValueModelBinding]
        [Route("purchaseorder/paymentterms")]
        public async Task<IActionResult> UploadPurchaseOrderPaymentTermsDocuments([FromQuery] Guid id)
        {
            UploadResult uploadResult = await this.ProcessUploadRequest($"{this.BlobPath}");
            if (!uploadResult.Success)
            {
                ModelStateDictionary state = (ModelStateDictionary)uploadResult;
                return BadRequest(state);
            }
            List<DocumentInfo> result = new List<DocumentInfo>();
            string userFullName = GetUserFullName();
            foreach (var item in uploadResult.Items)
            {
                Document doc = new Document { Id = Guid.NewGuid(), BlobUrl = item.BlobUrl, Name = item.OriginalFileName, CompanyId = this.OrganisationId, UploadedBy = userFullName };
                PurchaseOrderDocument pd = new PurchaseOrderDocument { PurchaseOrderId = id, DocumentId = doc.Id, DocumentType = PurchaseOrderDocumentType.PaymentTerms };

                _context.Documents.Add(doc);
                _context.PurchaseOrderDocuments.Add(pd);

                result.Add(doc.Adapt<DocumentInfo>());
            }

            _context.SaveChanges();
            return Created(nameof(DocumentController), result);
        }


        [HttpPost]
        [SwaggerResponse("201", typeof(List<DocumentInfo>), Description = "Created Result")]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(1073741274)]
        [Route("Product")]
        public async Task<IActionResult> Product([FromQuery] Guid id, ProductDocumentType documentType = ProductDocumentType.Specification, string description = null)
        {
            UploadResult uploadResult = await this.ProcessUploadRequest($"{this.BlobPath}");
            if (!uploadResult.Success)
            {
                ModelStateDictionary state = (ModelStateDictionary)uploadResult;
                return BadRequest(state);
            }
            List<DocumentInfo> result = new List<DocumentInfo>();
            string userFullName = GetUserFullName();
            foreach (var item in uploadResult.Items)
            {
                var extension = Path.GetExtension(item.OriginalFileName).ToLowerInvariant();
                Document doc = new Document { Id = Guid.NewGuid(), BlobUrl = item.BlobUrl, Name = item.OriginalFileName, Description = description, CompanyId = this.OrganisationId, IsRichText = false, UploadedBy = userFullName, Extension = extension };
                ProductDocument pd = new ProductDocument { ProductId = id, DocumentId = doc.Id, DocumentType = documentType };

                _context.Documents.Add(doc);
                _context.ProductDocuments.Add(pd);

                result.Add(doc.Adapt<DocumentInfo>());
            }

            await _context.SaveChangesAsync();

            bool hasNoDefaultImages = _context.ProductDocuments.Where(x => x.IsActive && x.ProductId == id).All(x => !x.IsDefaultImage);

            if (hasNoDefaultImages)
            {
                var firstImageDocument = _context.ProductDocuments
                    .Include(i => i.Document)
                    .Where(x => x.ProductId == id && PermittedImageExtensions.Contains(x.Document.Extension) && x.IsActive)
                    .OrderByDescending(x => x.Document.CreationDateInternal)
                    .FirstOrDefault();

                if (firstImageDocument != null)
                {
                    firstImageDocument.IsDefaultImage = true;
                }
            }

            await _context.SaveChangesAsync();

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                $"product_{documentType.ToString()}_updated",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id.ToString(), Type = ActivityTypeEnum.UPLOAD, Entity = ActivityEntityTypeEnum.PRODUCT, CustomDescription = documentType.ToString() } }
            );

            return Created(nameof(DocumentController), result);
        }

        [HttpPost]
        [SwaggerResponse("201", typeof(List<DocumentInfo>), Description = "Created Result")]
        [Route("ProductDuplicate")]
        public async Task<IActionResult> ProductDuplicate(ProductDuplicateRequest request)
        {
            foreach (var guid in request.documentIds)
            {
                ProductDocument originalProductDocument = _context.ProductDocuments.FirstOrDefault(pd => pd.DocumentId == guid);

                ProductDocument proddoc = new ProductDocument()
                {
                    DocumentId = guid,
                    ProductId = request.productId,
                    DocumentType = originalProductDocument.DocumentType
                };
                _context.ProductDocuments.Add(proddoc);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [SwaggerResponse("201", typeof(List<DocumentInfo>), Description = "Created Result")]
        [Route("ProductRichText")]
        public async Task<IActionResult> ProductRichText(Guid id, string description, string richTextData, ProductDocumentType documentType = ProductDocumentType.Specification)
        {
            string userFullName = GetUserFullName();
            Document doc = new Document { Id = Guid.NewGuid(), BlobUrl = "", Name = description, Description = description, CompanyId = this.OrganisationId, IsRichText = true, RichTextData = richTextData, UploadedBy = userFullName };

            ProductDocument pd = new ProductDocument { ProductId = id, DocumentId = doc.Id, DocumentType = documentType };

            _context.Documents.Add(doc);
            _context.ProductDocuments.Add(pd);

            DocumentInfo result = new DocumentInfo() 
            {
                Id = doc.Id,
                IsRichText = true,
                IsActive = true,
                Name = doc.Name,
                Description = doc.Description,
                RichTextData = doc.RichTextData,
                DocumentType = pd.DocumentType.ToString(),
                DateUploaded = doc.DateUploaded,
                UploadedBy = doc.UploadedBy
            };
            await _context.SaveChangesAsync();
            return Created(nameof(DocumentController), result);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("ProductRichText")]
        public async Task<IActionResult> GetProductRichTextDocuments([FromQuery] PagedResultParameters @params, Guid id, string search = null, ProductDocumentType documentType = ProductDocumentType.Specification)
        {
            var query = _context.ProductDocuments.Include(i => i.Document).Where(q =>
                q.ProductId == id &&
                q.IsActive &&
                q.Document.IsRichText &&
                (documentType == ProductDocumentType.All || q.DocumentType == documentType));

            if (search != null && search.Length > 1)
            {
                query = query.Where(q => EF.Functions.Like(q.Document.Description, $"%{search}%") || EF.Functions.Like(q.Document.Name, $"%{search}%"));
            }

            List<DocumentInfo> docList = new List<DocumentInfo>();

            foreach (var item in query)
            {
                docList.Add(new DocumentInfo
                {
                    Id = item.Document.Id,
                    IsRichText = item.Document.IsRichText,
                    IsActive = item.Document.IsActive,
                    Name = item.Document.Name,
                    Description = item.Document.Description,
                    DocumentType = item.DocumentType.ToString(),
                    RichTextData = item.Document.RichTextData,
                    DateUploaded = item.Document.DateUploaded,
                    UploadedBy = item.Document.UploadedBy
                });
            }
            
            IPagedList<DocumentInfo> documents = await docList.ToPagedListAsync(@params.PageNumber, @params.PageSize);
            
            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(documents));
        }

        [HttpGet]
        [Route("SingleProductRichText")]
        public async Task<IActionResult> GetSingleProductRichText(Guid id)
        {
            var doc = _context.Documents.FirstOrDefaultAsync(x => x.Id == id).Result;

            DocumentInfo docInfo = new DocumentInfo()
            {
                Name = doc.Name,
                Description = doc.Description,
                DateUploaded = doc.DateUploaded,
                Id = doc.Id,
                IsActive = doc.IsActive,
                IsRichText = doc.IsRichText,
                RichTextData = doc.RichTextData,
                UploadedBy = doc.UploadedBy
            };
            return Ok(docInfo);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("Product")]
        public async Task<IActionResult> GetProductDocuments([FromQuery] PagedResultParameters @params, Guid id, ProductDocumentType documentType = ProductDocumentType.Specification, bool? isRichText = null, string search = null)
        {
            IPagedList<DocumentInfo> documents = await _mediator.Send(new GetProductDocumentListQuery
            {
                ProductId = id,
                DocumentType = documentType,
                Search = search,
                PageNumber = @params.PageNumber,
                PageSize = @params.PageSize,
                UserEmail = GetUserEmail(),
                IsRichText = isRichText
            });
            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(documents));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("Product/Attached")]
        public async Task<IActionResult> GetProductAttachments([FromQuery] PagedResultParameters @params, Guid id, string search = null)
        {
            List<DocumentInfo> attachments = await _mediator.Send(new GetProductAttachmentsQuery
            {
                ProductId = id,
                UserEmail = GetUserEmail(),
            });
            return Ok( await attachments.Adapt<List<DocumentInfo>>().ToPagedListAsync(@params.PageNumber, @params.PageSize));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<DocumentInfo>), Description = "Ok")]
        [Route("QuotationRequest")]
        public async Task<IActionResult> GetQuotationRequestDocuments([FromQuery] PagedResultParameters @params, Guid id, string search = null)
        {
            List<DocumentInfo> documents = await _mediator.Send(new GetQuotationRequestProductDocumentsQuery
            {
                QuotationRequestId = id,
                Search = search
            });

            IPagedList<DocumentInfo> result = await documents.Adapt<List<DocumentInfo>>().ToPagedListAsync(@params.PageNumber, @params.PageSize);
            foreach (DocumentInfo doc in documents)
            {
                doc.Owner = doc.CreatedByUser == GetUserEmail();
            }

            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(result));
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(List<DocumentInfo>), Description = "Updated Result")]
        [Route("ProductMediaDefaultImage")]
        public async Task<IActionResult> Product([FromQuery] Guid documentId, Guid productId)
        {
            ProductDocument currentMainThumbnail = await _context.ProductDocuments.FirstOrDefaultAsync(x => x.IsDefaultImage && x.ProductId == productId);
            ProductDocument selectedMainThumbnail = await _context.ProductDocuments
                .Include(x => x.Document)
                .FirstOrDefaultAsync(x => x.DocumentId == documentId && x.ProductId == productId);

            if (selectedMainThumbnail == null)
            {
                return new NotFoundResult();
            }

            var extension = Path.GetExtension(selectedMainThumbnail.Document.Name).ToLowerInvariant();
            if (!PermittedImageExtensions.Contains(extension))
            {
                return new NotFoundResult();
            }

            if (currentMainThumbnail?.DocumentId == selectedMainThumbnail?.DocumentId || currentMainThumbnail != null)
            {
                currentMainThumbnail.IsDefaultImage = false;
            } 

            if (currentMainThumbnail?.DocumentId != selectedMainThumbnail?.DocumentId)
            {
                selectedMainThumbnail.IsDefaultImage = true;
            }


            await _context.SaveChangesAsync();

            IPagedList<DocumentInfo> documents = await _mediator.Send(new GetProductDocumentListQuery
            {
                ProductId = productId,
                DocumentType = ProductDocumentType.Media,
                Search = null,
                PageNumber = 1,
                PageSize = 10,
                UserEmail = GetUserEmail(),
                IsRichText = false
            });
            return this.HandleSuccessResponse(new ListResource<DocumentInfo>(documents));
        }

        private string GetUserFullName()
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string userFullName = identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();
            return userFullName;
        }

        private string GetUserEmail()
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string email = identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault();
            return email;
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : FilesController
    {
        public ImageController(ILogger<DocumentController> logger, TradefactDbContext context, IAzureBlobService blobService, IMimeMappingService mimeMappingService) : base(logger, context, blobService, mimeMappingService) { }
        public override string[] PermittedExtensions => this.PermittedImageExtensions.Concat(this.PermittedDocumentExtensions).ToArray();

        protected override string GetBlobPath()
        {
            return $"images";
        }
    }

}