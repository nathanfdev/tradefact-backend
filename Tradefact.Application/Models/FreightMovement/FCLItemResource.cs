using System.Collections.Generic;

namespace Tradefact.Application.Models
{
    public class FCLItemResource
    {
        public string ContainerType { get; set; }
        public string HazardCode { get; set; }

        public List<CargoItemResource> CargoItems { get; set; }
    }

}
