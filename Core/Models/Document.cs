using System;
using Core.Enums;

namespace Core.Models
{
    public class Document : BaseEntity<Document>
    {
        public string BlobUrl { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;
        public Guid CompanyId { get; set; }
        public Organisation Company { get; set; }
        public bool IsRichText { get; set; }
        public string RichTextData { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool ThumbnailGenerated { get; set; }

        public string UploadedBy { get; set; }
    }
}
