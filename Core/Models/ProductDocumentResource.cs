using Core.Enums;

namespace Core.Models
{
    public class ProductDocumentResource : Document
    {
        public int DocumentType { get; set; }

        public bool IsDefaultImage { get; set; }
    }
}
