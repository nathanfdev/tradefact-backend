using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Models
{
    public class AppSettings
    {
        public string B2CTenant { get; set; }
        public string B2CPolicy { get; set; }
        public string B2CClientId { get; set; }
        public string B2CRedirectUri { get; set; }
        public string B2CSignUpUrl { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
