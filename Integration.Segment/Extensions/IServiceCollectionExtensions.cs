using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Segment
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSegmentService(this IServiceCollection services)
        {
            services.AddTransient<IAnalyticsService, SegmentService>();
            return services;
        }
    }
}
