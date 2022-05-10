using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISeaTrackingService
    {
        Task<string> InitiateContainerTrack(List<string> containers, string carrier);
    }
}
