using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Service
{
    public interface IAzureServiceBusService
    {
        void SendInvite();
    }
}
