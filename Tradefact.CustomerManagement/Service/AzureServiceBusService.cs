using Core.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Service
{
    public class AzureServiceBusService : IAzureServiceBusService
    {
        private readonly IServiceBusClient _serviceBus;
        public AzureServiceBusService(IServiceBusClient client)
        {
            _serviceBus = client;
        }

        public void SendInvite()
        {

        }
    }
}
