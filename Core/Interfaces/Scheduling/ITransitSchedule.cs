using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces.Scheduling
{
    public interface ITransitSchedule
    {
        Task<TransitSchedule> GetScheduleAsync(TransitScheduleRequest request, CancellationToken cancellationToken = default);
    }
}
