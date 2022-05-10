using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tradefact.Data;

namespace FunctionApp.Image.Process
{
    public class ImageProcessor
    {
        protected readonly TradefactDbContext _context;

        public ImageProcessor(TradefactDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        static string s_storageAccountConnectionString = System.Environment.GetEnvironmentVariable("tradefactBlobImageStorage");

        private static CloudBlobClient _blobClient = GetBlobClient();

        private static CloudBlobClient GetBlobClient()
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(s_storageAccountConnectionString);

            // Create the blob client.
            return storageAccount.CreateCloudBlobClient();
        }


        [FunctionName("ProcessBlobEvent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var validationEventType = "Microsoft.EventGrid.SubscriptionValidationEvent";

            log.LogInformation("C# HTTP trigger function processed a request.");
            string response = string.Empty;
            string requestContent = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"Received events: {requestContent}");
            if (requestContent?.Length > 0)
            {
                EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();

                EventGridEvent[] eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);

                foreach (EventGridEvent eventGridEvent in eventGridEvents)
                {
                    if (eventGridEvent.Data is SubscriptionValidationEventData)
                    {
                        var eventData = (SubscriptionValidationEventData)eventGridEvent.Data;
                        return new OkObjectResult(new {
                            validationResponse = eventData.ValidationCode
                        });
                    }

                    if (eventGridEvent.Data is StorageBlobCreatedEventData)
                    {
                        var eventData = (StorageBlobCreatedEventData)eventGridEvent.Data;
                        await GenerateThumbnail(eventData, log);

                        log.LogInformation($"Got StorageBlobCreatedEventData event data, url: {eventData.Url}, topic: {eventGridEvent.Topic}");
                    }
                }
            }
            return new OkObjectResult(response);
        }

        private async Task GenerateThumbnail(StorageBlobCreatedEventData eventdata, ILogger log) {


            var url = eventdata.Url.ToString();
            // Check this event is not related to a thumbnail
            if (url.Contains("_thumb"))
                return;

            var extension = Path.GetExtension(url);
            var encoder = GetEncoder(extension);

            CloudBlob cblob = GetCloudBlobFromUrl(url);

            string blobName = cblob.Name;
            CloudBlobContainer container = _blobClient.GetContainerReference(cblob.Container.Name);

            // Create reference to a blob named "blobName".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            var inputStream = await blockBlob.OpenReadAsync();


            CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference(blobName.Replace(extension, $"_thumb{extension}"));
            if (await blockBlobThumb.ExistsAsync())
                return;

            if (encoder != null)
            {
                var thumbnailWidth = 60; //Convert.ToInt32(Environment.GetEnvironmentVariable("THUMBNAIL_WIDTH"));
                var thumbnailHeight = 60;
                var thumbContainerName = Environment.GetEnvironmentVariable("THUMBNAIL_CONTAINER_NAME");

                using (var output = new MemoryStream())
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(inputStream) as Image<Rgba32>)
                {
                    if (image.Width > thumbnailWidth)
                    {
                        var divisor = image.Width / thumbnailWidth;
                        var height = Convert.ToInt32(Math.Round((decimal)(image.Height / divisor)));

                        //image.Mutate(x => x.Resize(thumbnailWidth, height));
                        image.Mutate(x => x.Resize(thumbnailWidth, thumbnailHeight));
                        image.Save(output, encoder);
                        output.Position = 0;

                        await blockBlobThumb.UploadFromStreamAsync(output);

                        await MarkThumbnailCreated(cblob.Uri.ToString(), blockBlobThumb.Uri.ToString());

                        blockBlobThumb.Properties.ContentType = "image/jpeg";
                        await blockBlobThumb.SetPropertiesAsync();
                    }
                }
            }
            else
            {
                log.LogInformation($"No encoder support for: {eventdata.Url}");
            }

        }

        private async Task MarkThumbnailCreated(string bloblUrl, string thumbnailbloblUrl)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(q=>q.BlobUrl == bloblUrl);
            if (document != null)
            {
                document.ThumbnailUrl = thumbnailbloblUrl;
                document.ThumbnailGenerated = true;

                _ = await _context.SaveChangesAsync();
            }
        }


        private CloudBlob GetCloudBlobFromUrl(string bloblUrl)
        {
            var myUri = new Uri(bloblUrl);
            return new CloudBlob(myUri);
        }

        private IImageEncoder GetEncoder(string extension)
        {
            IImageEncoder encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension.ToLower())
                {
                    case "png":
                        encoder = new PngEncoder();
                        break;
                    case "jpg":
                        encoder = new JpegEncoder();
                        break;
                    case "jpeg":
                        encoder = new JpegEncoder();
                        break;
                    case "gif":
                        encoder = new GifEncoder();
                        break;
                    case "bmp":
                        encoder = new BmpEncoder();
                        break;
                    default:
                        break;
                }
            }
            return encoder;
        }

    }
}
