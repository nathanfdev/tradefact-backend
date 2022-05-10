using Core.Enums;
using Core.Interfaces;
using Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Tradefact.Data;
using Tradefact.Services;
using Tradefact.Utilities.ActivationDataSetup.Services;

namespace Tradefact.Utilities.ActivationDataSetup
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

            var setupService = serviceProvider.GetService<IActivationDataSetupService>();

            try
            {
                if (args.Length > 0)
                {
                    setupService.SetFilter(args[0]);
                }

                setupService.FixInvitations(OrganisationTypeEnum.SHIPPER, InviteType.NewShipper);
                setupService.FixInvitations(OrganisationTypeEnum.PARTNER, InviteType.NewFreightForwarderInvitedByShipper);
                setupService.SetUpTradefactLogistics();
                setupService.SaveChanges();
                setupService.QueueActivations();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            logger.LogDebug("Finished");
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TradefactDbContext>(opts =>
            {
                string dbConnectionString = configuration.GetConnectionString("TradefactDB");
                opts.UseSqlServer(dbConnectionString, b => b.MigrationsAssembly("Tradefact.Api"));
            });

            services
                .AddLogging(b => b.AddFilter("Microsoft", LogLevel.Warning).AddConsole())
                .AddSingleton<IUserResolverService, UserResolverService>(b => new UserResolverService("logistics@tradefact.com"))
                .AddSingleton<IActivationDataSetupService, ActivationDataSetupService>();

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
