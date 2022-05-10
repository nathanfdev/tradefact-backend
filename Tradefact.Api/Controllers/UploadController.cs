using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Tradefact.Api.Infrastructure.Helpers;
using Tradefact.Api.Model;
using Tradefact.Data;
using Tradefact.Services;

namespace Tradefact.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public abstract class FilesController : BaseApiController
    {
        private readonly ILogger _logger;

        Dictionary<string, long> FileSizeLimit = new Dictionary<string, long> 
        {
            { "image", 20971520 },
            { "audio", 20971520 },
            { "video", 1073741274 }
        };

        protected readonly string[] PermittedImageExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
        protected readonly string[] PermittedDocumentExtensions = { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt", ".csv", ".pdf", ".ppt", ".pptx", ".aac", ".mp3", ".wav", ".wma", ".ac3", ".dts", ".mpg", ".mpeg", ".mp4", ".avi", ".mov", ".m2ts" };

        private static readonly List<MyFormModel> MyFiles = new List<MyFormModel>();
        protected readonly IAzureBlobService _blobService;
        private readonly IMimeMappingService _mimeMappingService;

        public FilesController(ILogger logger, TradefactDbContext context, IAzureBlobService blobService, IMimeMappingService mimeMappingService) : base(context)
        {
            _logger = logger;
            _blobService = blobService;
            _mimeMappingService = mimeMappingService;

            //_targetFolderPath = config.GetValue<string>("StoredFilesPath");   // To save physical files to a path provided by configuration:
            ////_targetFolderPath = Path.GetTempPath(); // To save physical files to the temporary files folder
            //Directory.CreateDirectory(_targetFolderPath);
        }

        public virtual string[] PermittedExtensions => this.PermittedDocumentExtensions;
        public string ContainerName => this.OrganisationId.ToString("N");
        public string BlobPath => this.GetBlobPath();

        protected virtual string GetBlobPath()
        {
            return this.OrganisationId.ToString("N");
        }

        protected async Task<UploadResult> ProcessUploadRequest(string blobPath)
        {
            UploadResult result = new UploadResult();


            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                result.Success = false;
                result.AddModelError("File", "The request couldn't be processed (Error 1).");
                result.AddModelError("ContentType", $"The request content type [{Request.ContentType}] is invalid.");
                _logger.LogInformation($"The request content type [{Request.ContentType}] is invalid.");

                return result;
            }

            var formModel = new MyFormModel();

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), new FormOptions().MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (contentDisposition.IsFileDisposition())
                    {
                        UploadItem doc = new UploadItem();

                        // Don't trust the file name sent by the client. To display the file name, HTML-encode the value.
                        string original_extension = Path.GetExtension(contentDisposition.FileName.Value);
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), original_extension);

                        var contentType = _mimeMappingService.Map(contentDisposition.FileName.Value);

                        doc.OriginalFileName = trustedFileNameForDisplay;
                        doc.ContentType = contentType;

                        string fileType = contentType.Split("/")[0];

                        var sizeLimit = FileSizeLimit.ContainsKey(fileType) ? FileSizeLimit[fileType] : 20971520;

                        // todo: scan the file's contents using an anti-virus/anti-malware scanner API
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition, result, PermittedExtensions, sizeLimit);
                        if (!result.IsValid)
                        {
                            result.Success = false;
                            return result;
                        }

                        //// 3b. save the file on Path
                        MemoryStream stream = new MemoryStream(streamedFileContent);
                        var blobUrl = await _blobService.UploadFromStreamAsync(ContainerName, $"{blobPath}/{trustedFileNameForFileStorage}", stream, contentType);
                        result.BlobUrls.Add(blobUrl);

                        doc.BlobUrl = blobUrl;
                        result.Items.Add(doc);

                        _logger.LogInformation($"Uploaded file '{trustedFileNameForDisplay}' saved to '{blobUrl}' as {trustedFileNameForFileStorage}");
                    }
                    else if (contentDisposition.IsFormDisposition())
                    {
                        var content = new StreamReader(section.Body).ReadToEnd();
                        if (contentDisposition.Name == "userId" && int.TryParse(content, out var useId))
                        {
                            formModel.UserId = useId;
                        }

                        if (contentDisposition.Name == "comment")
                        {
                            formModel.Comment = content;
                        }

                        if (contentDisposition.Name == "isPrimary" && bool.TryParse(content, out var isPrimary))
                        {
                            formModel.IsPrimary = isPrimary;
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // todo: validate and persist formModel
            _logger.LogInformation(formModel.ToString());
            return result;
        }

        //[HttpGet("")]
        //public async Task<IActionResult> Download([FromQuery]string guid)
        //{
        //    var myFile = MyFiles.FirstOrDefault(x => x.Guid == guid);
        //    if (myFile == null)
        //    {
        //        _logger.LogInformation($"File with GUID={guid} Not Found.");
        //        return BadRequest($"File with GUID={guid} Not Found.");
        //    }

        //    var filePath = myFile.TrustedFilePath;
        //    _logger.LogInformation($"downloading file [{filePath}].");
        //    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        //    return File(bytes, GetMimeTypes(Path.GetExtension(myFile.TrustedFileName)), myFile.TrustedFileName);
        //}

        private static string GetMimeTypes(string ext)
        {
            switch (ext)
            {
                case ".txt": return "text/plain";
                case ".csv": return "text/csv";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/vnd.ms-word";
                case ".xls": return "application/vnd.ms-excel";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                default: return "application/octet-stream";
            }
        }
    }

    public class MyFormModel
    {
        public string TrustedFilePath { get; set; }
        public string TrustedFileName { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public string Guid { get; private set; } = System.Guid.NewGuid().ToString();
        public bool IsPrimary { get; set; }

        public override string ToString()
        {
            return $"{nameof(TrustedFilePath)}: [{TrustedFilePath}];" + Environment.NewLine +
                   $"{nameof(UserId)}: {UserId}; " + Environment.NewLine +
                   $"{nameof(Guid)}: {Guid}; " + Environment.NewLine +
                   $"{nameof(IsPrimary)}: {IsPrimary}; ";
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }

}