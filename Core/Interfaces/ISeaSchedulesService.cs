using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISeaSchedulesService
    {
        Task<RouteScheduleResult> GetRouteSchedule(RouteScheduleCriteria criteria);

        Dictionary<string, string> GetOptions();
    }
}
