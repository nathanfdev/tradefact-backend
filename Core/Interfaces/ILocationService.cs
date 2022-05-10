using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILocationService
    {
        Task<GeographicPosition> GetAddressLocation(Address address);
        Dictionary<string, string> GetOptions();
    }
}
