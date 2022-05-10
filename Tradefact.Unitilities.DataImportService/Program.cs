using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Tradefact.Data;
using Tradefact.Utilities.DataImport.Services;

namespace Tradefact.Utilities.DataImport
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            //setup our DI
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogDebug("Starting application");

            Console.WriteLine(configuration.GetConnectionString("TradefactDB"));

            var import_service = serviceProvider.GetService<IReferenceDataService>();

            try
            {
                import_service.ImportThomasPinkProductData();
                //import_service.ImportContainers();
                //import_service.ImportPorts();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            logger.LogDebug("Application Closing");
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TradefactDbContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("TradefactDB"), b => b.MigrationsAssembly("Tradefact.Api")));

            services.AddLogging(b => b.AddConsole())
                .AddSingleton<IReferenceDataService, ReferenceDataService>()
                .AddSingleton<IUserResolverService, ImportUserService>(b => new ImportUserService("System.Import"))
                .BuildServiceProvider();
        }

    }
}
