using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Integration.Segment.Extensions;
using Microsoft.Extensions.Logging;
using Segment;
using Segment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Integration.Segment
{
    public class SegmentService : IAnalyticsService
    {
        private readonly ILogger<SegmentService> _logger;
        private readonly SegmentOptions _options;

        public SegmentService(ILogger<SegmentService> logger, SegmentOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            Analytics.Initialize(options.WriteKey);
        }

        public Dictionary<string, string> GetOptions()
        {
            return (Dictionary<string, string>)_options.AsDictionary();
        }

        private async Task TrackEvent(string userId, string orgId, string eventName, object eventProps, Traits userTraits = null, Traits groupTraits = null)
        {
            await Task.Run(() =>
            {
                if (userTraits == null) userTraits = new Traits();
                if (groupTraits == null) groupTraits = new Traits();

                var options = new Options().SetContext(new Context { { "groupId", orgId } }); // Totango

                // Identify
                Analytics.Client.Identify(userId, userTraits, options);

                // Group
                Analytics.Client.Group(userId, orgId, groupTraits, options);

                // Track
                IDictionary<string, object> properties = (eventProps == null) ? 
                    new Dictionary<string, object>() :
                    ReflectionExtensions.AsDictionary(eventProps);

                if (!properties.ContainsKey("category"))
                {
                    properties.Add("category", "api"); // Totango
                }

                Analytics.Client.Track(userId, eventName, properties, options);
            });
        }

        public async Task TrackEvent(ClaimsPrincipal user, Organisation org, string eventName, object eventProps = null)
        {
            ClaimsIdentity identity = (ClaimsIdentity)user.Identity;

            await TrackEvent(
                identity.Claims.Where(c => c.Type == "InternalUserId").Select(c => c.Value).SingleOrDefault(),
                org.Id.ToString(),
                eventName,
                eventProps,
                new Traits()
                    .AddIfNotNull("name", identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault())
                    .AddIfNotNull("email", identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault()),
                new Traits()
                    .AddIfNotNull("name", org.Name)
                    .AddIfNotNull("organisation_type", org.OrganisationTypeId.ToString()));
        }

        public async Task TrackEvent(ClaimsPrincipal user, Guid orgId, string eventName, object eventProps = null)
        {
            ClaimsIdentity identity = (ClaimsIdentity)user.Identity;

            await TrackEvent(
                identity.Claims.Where(c => c.Type == "InternalUserId").Select(c => c.Value).SingleOrDefault(),
                orgId.ToString(),
                eventName,
                eventProps,
                new Traits()
                    .AddIfNotNull("name", identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault())
                    .AddIfNotNull("email", identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault()));
        }

        public async Task TrackEvent(string userId, string userFullName, string userEmail, Organisation org, string invitedBy, string eventName, object eventProps = null)
        {
            await TrackEvent(
                userId,
                org.Id.ToString(),
                eventName,
                eventProps,
                new Traits()
                    .AddIfNotNull("name", userFullName)
                    .AddIfNotNull("email", userEmail),
                new Traits()
                    .AddIfNotNull("name", org.Name)
                    .AddIfNotNull("organisation_type", org.OrganisationTypeId.ToString())
                    .AddIfNotNull("invited_by", invitedBy));
        }

        public async Task TrackAnonymousEvent(Guid anonymousId, string name, string email, string companyName, string eventName, object eventProps = null)
        {
            await TrackEvent(
                anonymousId.ToString(), "$NOGROUP", eventName, eventProps,
                new Traits()
                    .AddIfNotNull("name", $"{name} ({companyName})")
                    .AddIfNotNull("email", email));
        }

        public async Task TrackOrganisationMetric(ClaimsPrincipal user, Guid orgId, string metric, object value)
        {
            ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
            string userId = identity.Claims.Where(c => c.Type == "InternalUserId").Select(c => c.Value).SingleOrDefault();

            await Task.Run(() =>
            {
                var groupTraits = new Traits { { metric, value} };
                Analytics.Client.Group(userId, orgId.ToString(), groupTraits);
            });
        }
    }
}
