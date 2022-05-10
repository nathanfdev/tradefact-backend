using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tradefact.Services
{
    public class AzureStorageSettings
    {
        public string AzureStorage { get; set; }
        public bool EnableAzureQueueService { get; set; }
        public bool EnableAzureBlobImageService { get; set; }
    }
    public interface IAzureBlobService
    {
        Task<string> UploadFormFileAsync(string containerName, string blobName, IFormFile file);
        Task<string> UploadFromStreamAsync(string containerName, string blobName, Stream stream, string contentType);
        Task DeleteImageAsync(string containerName, string blobUrl);
        CloudBlockBlob GetBlockBlob(string containerName, string blobUrl);
    }

    public interface IBlockBlob
    {
        Task<string> UploadFormFileAsync(string containerName, string blobName, IFormFile file);

        Task<string> UploadFromStreamAsync(string containerName, string blobName, Stream stream, string contentType);

        Task DeleteAsync(string containerName, string blobUrl);

        CloudBlockBlob GetBlockBlob(string containerName, string blobUrl);
    }

    public class BlockBlob : IBlockBlob
    {
        private readonly AzureStorageSettings _storageSettings;

        public BlockBlob(AzureStorageSettings storageSettings)
        {
            this._storageSettings = storageSettings;
        }

        public async Task<string> UploadFormFileAsync(string containerName, string blobName, IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                return await this.UploadFromStreamAsync(containerName, blobName, stream, file.ContentType);
            }
        }

        public async Task<string> UploadFromStreamAsync(string containerName, string blobName, Stream stream, string contentType)
        {
            var container = CloudStorageAccount.Parse(_storageSettings.AzureStorage).CreateCloudBlobClient().GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, new BlobRequestOptions(), new OperationContext());
            var blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.DeleteIfExistsAsync();

            blockBlob.Properties.ContentType = contentType;

            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri.ToString();
        }

        public async Task DeleteAsync(string containerName, string blobUrl)
        {
            var blobContainer = CloudStorageAccount.Parse(_storageSettings.AzureStorage).CreateCloudBlobClient().GetContainerReference(containerName);
            var blobName = blobUrl.Replace($"{blobContainer.Uri.AbsoluteUri}/", string.Empty);
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);

            await blockBlob.DeleteAsync();
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string blobUrl)
        {
            var blobContainer = CloudStorageAccount.Parse(_storageSettings.AzureStorage).CreateCloudBlobClient().GetContainerReference(containerName);
            var blobName = blobUrl.Replace($"{blobContainer.Uri.AbsoluteUri}/", string.Empty);
            return blobContainer.GetBlockBlobReference(blobName);
        }
    }

    public class AzureBlobService : IAzureBlobService
    {
        private readonly IBlockBlob blockBlob;

        public AzureBlobService(IBlockBlob blockBlob)
        {
            this.blockBlob = blockBlob;
        }

        public async Task DeleteImageAsync(string containerName, string blobUrl)
        {
            await blockBlob.DeleteAsync(containerName, blobUrl);
        }

        public async Task<string> UploadFormFileAsync(string containerName, string blobName, IFormFile file)
        {
            return await blockBlob.UploadFormFileAsync(containerName, blobName, file);
        }

        public async Task<string> UploadFromStreamAsync(string containerName, string blobName, Stream stream, string contentType)
        {
            return await blockBlob.UploadFromStreamAsync(containerName, blobName, stream, contentType);
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string blobUrl)
        {
            return blockBlob.GetBlockBlob(containerName, blobUrl);
        }
    }

    //public interface IDocumentService
    //{
    //    Task<string> UploadDocumentAsync(Guid organizationId, IFormFile attachment);
    //    Task DeleteDocumentAsync(string documentUrl);
    //}

    //public class DocumentService : IDocumentService
    //{
    //    private const string ContainerName = "documents";
    //    private readonly IBlockBlob blockBlob;

    //    public DocumentService(IBlockBlob blockBlob)
    //    {
    //        this.blockBlob = blockBlob;
    //    }

    //    public async Task<string> UploadDocumentAsync(Guid organizationId, IFormFile attachment)
    //    {
    //        var blobPath = "organizationId/" + organizationId;
    //        return await UploadAttachmentAsync(blobPath, attachment);
    //    }

    //    public async Task DeleteDocumentAsync(string documentUrl)
    //    {
    //        await blockBlob.DeleteAsync(ContainerName, documentUrl);
    //    }

    //    private async Task<string> UploadAttachmentAsync(string blobPath, IFormFile attachment)
    //    {
    //        var fileName = ContentDispositionHeaderValue.Parse(attachment.ContentDisposition).FileName.ToString().Trim('"').ToLower();
    //        var blobName = blobPath + "/" + fileName;
    //        return await blockBlob.UploadFromStreamAsync(ContainerName, blobName, attachment);
    //    }
    //}

}
