using Microsoft.Extensions.DependencyInjection;
using Core.Interfaces;
using System;
using Integration.VTExplorer.Services;

namespace Integration.VTExplorer.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddVTExplorerSeaTrackingService(this IServiceCollection services)
        {
            services.AddHttpClient("vtExplorer", c =>
            {
                c.BaseAddress = new Uri("https://api.vtexplorer.com");
            });

            services.AddTransient<IAISTrackingService, VTExplorerAISTrackingService>();
            return services;
        }
    }
}
