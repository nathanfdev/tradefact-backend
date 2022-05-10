using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Integration.CargoSmart
{
    public static class IServiceCollectionExtensions
    {

        public static IServiceCollection AddCargoSmartService(this IServiceCollection services)
        {
            services.AddHttpClient("cargosmart", c =>
            {
                //c.BaseAddress = new Uri("https://apis.cargosmart.com/openapi");
            });

            services.AddTransient<ISeaSchedulesService, CargosSmartService>();
            return services;
        }
    }
}
