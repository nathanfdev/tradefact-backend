using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models.Network
{
    public class NetworkResource
    {
        public List<NetworkInfoResource> RecentNetworks { get; set; }
        public NetworkCountResource Company { get; set; }
        public NetworkCountResource Logistics { get; set; }
    }

    public class NetworkInfoResource
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime CreationDateInternal { get; set; }
        public OrganisationTypeEnum OrganisationType { get; set; }
    }

    public class NetworkCountResource
    {
        public int PendingTotal { get; set; }
        public int ConnectedTotal { get; set; }
    }
}
