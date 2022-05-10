using System;
using System.IO;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using FunctionApp.Imports.Requests;
using Infrastructure.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Imports.Functions
{
    public class ImportDocumentFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public ImportDocumentFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(ImportDocument), Description = "OK result")]
        [SwaggerFormDataFile(true, "file", "Document to be uploaded")]
        [SwaggerFormData("companyId", false, typeof(string), "companyId")]
        [SwaggerFormData("importId", false, typeof(int), "importId")]
        [SwaggerFormData("ownedBy", false, typeof(OwnershipEnum), "ownedBy")]
        [Consumes("multipart/form-data")]
        [FunctionName(nameof(AddDocument))]
        public async Task<IActionResult> AddDocument(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger,
            [Blob("%AzureStorageBlobOptions:BlobPath%", FileAccess.ReadWrite, Connection = "AzureStorageBlobOptions:ConnectionString")]
        CloudBlobContainer cloudBlobContainer)

        {
            var formdata = await req.ReadFormAsync();

            string companyId = formdata["companyId"];
            string importId = formdata["importId"];
            string ownedBy = formdata["ownedBy"];

            var file = req.Form.Files["file"];
            var extension = Path.GetExtension(file.FileName);
            var blobName =
                $"{Guid.NewGuid()}{extension}";

            await cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            var cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference(blobName);

            cloudBlockBlob.Properties.ContentType = file.ContentType;

            using(var fileStream = file.OpenReadStream()) {
                await cloudBlockBlob.UploadFromStreamAsync(fileStream).ConfigureAwait(false);
            }

            var item = new ImportDocument
            {
                Id = Guid.NewGuid(),
                ImportId = importId,
                OwnedBy = (OwnershipEnum)Enum.Parse(typeof(OwnershipEnum), ownedBy, true),
                Name = file.FileName,
                Extension = extension,
                BlobName = blobName,
                CompanyId = companyId
            };

            item = await _repository.CreateAsync(item, _proxy.Imports).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [SwaggerResponse(200, null, Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DeleteDocumentRequest))]
        [FunctionName(nameof(DeleteDocument))]
        public async Task<IActionResult> DeleteDocument([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]

            HttpRequest req, ILogger logger)
        {
            var request =
                   await req.GetJsonBody<DeleteDocumentRequest, DeleteDocumentRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var documentId = request.Value.DocumentId;

            var importDocument = await _repository.GetAsync<ImportDocument>(documentId, companyId, _proxy.Imports)
                .ConfigureAwait(false);

            var blobName = importDocument.BlobName;

            var cloudStorageAccount =
                CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureStorageBlobOptions:ConnectionString"));

            var cloudBlobClient =
                cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer =
                cloudBlobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("AzureStorageBlobOptions:BlobPath"));

            var cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference(blobName);

            await cloudBlockBlob.DeleteAsync().ConfigureAwait(false);

            importDocument = await _repository.DeactivateAsync<ImportDocument>(documentId, companyId, _proxy.Imports)
                .ConfigureAwait(false);

            return new OkObjectResult(importDocument);
        }

        [SwaggerResponse(200, typeof(FileContentResult), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(DownloadDocumentRequest))]
        [FunctionName(nameof(DownloadDocument))]
        public async Task<IActionResult> DownloadDocument([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<DownloadDocumentRequest, DownloadDocumentRequestValidator>(logger)
                .ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var documentId = request.Value.DocumentId;

            //TODO: Pass SAS token and url back to client instead of loading into a memory stream
            var importDocument = await _repository.GetAsync<ImportDocument>(documentId, companyId, _proxy.Imports)
                .ConfigureAwait(false);

            var blobName = importDocument.BlobName;

            var cloudStorageAccount =
                CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureStorageBlobOptions:ConnectionString"));

            var cloudBlobClient =
                cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer =
                cloudBlobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("AzureStorageBlobOptions:BlobPath"));

            await cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            var cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference(blobName);

            if(cloudBlockBlob != null) {
                var ms = new MemoryStream();

                await cloudBlockBlob.DownloadToStreamAsync(ms).ConfigureAwait(false);

                return new FileContentResult(ms.ToArray(), cloudBlockBlob.Properties.ContentType);
            } else {
                return new NotFoundResult();
            }
        }

        [SwaggerResponse(200, typeof(ImportDocument[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(ListDocumentsRequest))]
        [FunctionName(nameof(ListDocuments))]
        public async Task<IActionResult> ListDocuments([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<ListDocumentsRequest, ListDocumentsRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var companyId = request.Value.CompanyId;
            var importId = request.Value.ImportId;
            var isActive = request.Value.IsActive;
            var ownedBy = request.Value.Ownership;

            var items = await _repository.GetAsync<ImportDocument>(
                                  c => (c.ImportId == importId) && (c.OwnedBy == ownedBy) && (c.IsActive == isActive),
                                  _proxy.Imports)
                .ConfigureAwait(false);

            return new OkObjectResult(items);
        }
    }
}