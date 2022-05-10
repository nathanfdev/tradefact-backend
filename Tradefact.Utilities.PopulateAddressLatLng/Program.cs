using Core.Interfaces;
using Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Tradefact.Data;
using Tradefact.Services;
using Integration.Here;

namespace Tradefact.Utilities.PopulateAddressLatLng
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            //setup our DI
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogDebug("Starting");
            logger.LogDebug(configuration.GetConnectionString("TradefactDB"));

            var service = serviceProvider.GetService<IPopulateAddressLatLngService>();
            service.GeocodeAddresses().Wait();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TradefactDbContext>(opts =>
            {
                string dbConnectionString = configuration.GetConnectionString("TradefactDB");
                opts.UseSqlServer(dbConnectionString, b => b.MigrationsAssembly("Tradefact.Api"));
            });

            var here_config = new HereOptions();
            configuration.Bind("here", here_config);      //  <--- This
            services.AddSingleton(here_config);
            services.AddHereService();

            services
                .AddLogging(b => b.AddFilter("Microsoft", LogLevel.Warning).AddConsole())
                .AddSingleton<IUserResolverService, UserResolverService>(b => new UserResolverService("logistics@tradefact.com"))
                .AddSingleton<IPopulateAddressLatLngService, PopulateAddressLatLngService>();

            services.AddSingleton<IServiceBusPersistedConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ServiceBusPersistedConnection>>();

                string serviceBusConnectionString = configuration.GetConnectionString("ServiceBus");
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                return new ServiceBusPersistedConnection(serviceBusConnection, logger);
            });

            services
                .AddTransient<IServiceBusClient, AzureServiceBusClient>()
                .BuildServiceProvider();
        }
    }
}
