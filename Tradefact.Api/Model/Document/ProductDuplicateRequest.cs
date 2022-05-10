using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Document
{
    public class ProductDuplicateRequest
    {
        public Guid productId { get; set; }
        public List<Guid> documentIds { get; set; }
    }
}
