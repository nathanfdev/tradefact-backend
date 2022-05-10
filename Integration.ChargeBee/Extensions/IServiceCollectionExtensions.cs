using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.ChargeBee
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddChargeBeeService(this IServiceCollection services)
        {
            services.AddTransient<IBillingService, ChargeBeeService>();
            return services;
        }
    }
}
