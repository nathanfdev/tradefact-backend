using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model
{
    public class UploadResult: ModelStateDictionary
    {
        public bool Success { get; set; } = true;
        public List<string> BlobUrls { get; set; } = new List<string>();
        public List<UploadItem> Items { get; set; } = new List<UploadItem>();
    }

    public class UploadItem
    {
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public string BlobUrl { get; set; }
    }

}
