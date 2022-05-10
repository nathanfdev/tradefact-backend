using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Document
{
    public class DocumentUploadRequest
    {
        public Guid ResourceId { get; set; }
        public string Name { get; set; }
    }

}
