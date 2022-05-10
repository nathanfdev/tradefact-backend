using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore;

namespace Core.Dtos
{
    public partial class DocumentInfo
    {
        [JsonProperty("blobUrl")]
        public string BlobUrl { get; set; }

        [JsonProperty("thumbnailBlobUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("dateUploaded")]
        public DateTimeOffset DateUploaded { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("isRichText")]
        public bool IsRichText { get; set; }

        [JsonProperty("source")]
        public string Src { get; set; }

        [JsonProperty("owner")]
        public bool Owner { get; set; }

        [JsonProperty("richTextData")]
        public string RichTextData { get; set; }

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("uploadedBy")]
        public string UploadedBy { get; set; }

        [JsonIgnore]
        public string CreatedByUser { get; set; }

        [JsonProperty("isDefaultImage")]
        public bool IsDefaultImage { get; set; }
    }
}
