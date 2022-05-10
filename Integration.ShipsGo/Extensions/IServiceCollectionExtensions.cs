using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace Integration.ShipsGo
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddShipsGoService(this IServiceCollection services)
        {
            services.AddTransient<ISeaTrackingService, ShipsGoService>();
            return services;
        }
    }
}
