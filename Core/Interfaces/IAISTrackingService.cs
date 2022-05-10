using Core.Models.Tracking;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAISTrackingService
    {
        Task<AISTrackingResult> GetVesselPosition(AISTrackCriteria criteria);
    }

    public class AISTrackingDisabledService: IAISTrackingService
    {
        public async Task<AISTrackingResult> GetVesselPosition(AISTrackCriteria criteria) {
            return new AISTrackingResult();
        }
    }
}
