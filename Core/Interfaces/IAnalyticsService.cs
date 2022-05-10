using Core.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAnalyticsService
    {
        Task TrackEvent(ClaimsPrincipal user, Organisation org, string eventName, object eventProps = null);

        Task TrackEvent(ClaimsPrincipal user, Guid orgId, string eventName, object eventProps = null);

        Task TrackEvent(string userId, string userFullName, string userEmail, Organisation org, string invitedBy, string eventName, object eventProps = null);

        Task TrackAnonymousEvent(Guid anonymousId, string name, string email, string company, string eventName, object eventProps = null);

        Task TrackOrganisationMetric(ClaimsPrincipal user, Guid orgId, string metric, object value);

        Dictionary<string, string> GetOptions();
    }
}
