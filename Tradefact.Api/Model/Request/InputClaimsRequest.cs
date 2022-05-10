using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model
{
    public class InputClaimsRequest
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
