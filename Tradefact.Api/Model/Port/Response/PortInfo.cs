using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Port.Response
{
    public class PortInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
    }
}
