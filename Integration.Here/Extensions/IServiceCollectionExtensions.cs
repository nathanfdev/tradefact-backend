using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Here
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHereService(this IServiceCollection services)
        {
            services.AddHttpClient("here", c => { });
            services.AddTransient<ILocationService, HereService>();

            return services;
        }
    }
}
