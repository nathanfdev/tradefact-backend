using Core.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITradefactActivityService
    {
        Task TrackEvent(ClaimsPrincipal user, Organisation org, string eventName, TrackWith analytics, EventProps eventProps);

        Task TrackEvent(ClaimsPrincipal user, Guid orgId, string eventName, TrackWith analytics, EventProps eventProps);

        Task TrackEvent(string userId, string userFullName, string userEmail, Organisation org, Organisation invitingOrg, string eventName, TrackWith analytics, EventProps eventProps);

        Task TrackAnonymousEvent(Guid anonymousId, string name, string email, string company, string eventName, TrackWith analytics, EventProps eventProps);
    }
}
