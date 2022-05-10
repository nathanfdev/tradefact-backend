using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAirSchedulesService
    {
        Task<RouteScheduleResult> GetRouteSchedule(RouteScheduleCriteria criteria);

        Dictionary<string, string> GetOptions();
    }
}
